using System.Collections.ObjectModel;
using System.Data;
using System.Linq.Expressions;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;

internal sealed class EstablishmentsSearchServiceAdapter
    : ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>
{
    private readonly EducationProviderRegistryDbContext _dbContext;
    private readonly ISearchTermRule<IQueryable<Establishment>> _searchTermRule;
    private readonly IFacetAggregator _facetCalculator;
    private readonly IMapper<
        (IReadOnlyList<EstablishmentReadModel>, IReadOnlyList<AggregatedFacetResult>),
        SearchResults<EstablishmentSearchResults, SearchFacets>> _searchResultsFromContextMapper;
    private readonly IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>> _searchFilterRequestMapper;
    private readonly ISearchFilterExpressionsBuilder<Establishment> _searchFilterExpressionsBuilder;

    public EstablishmentsSearchServiceAdapter(
        EducationProviderRegistryDbContext dbContext,
        ISearchTermRule<IQueryable<Establishment>> searchTermRule,
        IFacetAggregator facetCalculator,
        IMapper<
            (IReadOnlyList<EstablishmentReadModel>, IReadOnlyList<AggregatedFacetResult>),
            SearchResults<EstablishmentSearchResults, SearchFacets>> searchResultsFromContextMapper,
        IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>> searchFilterRequestMapper,
        ISearchFilterExpressionsBuilder<Establishment> searchFilterExpressionsBuilder)
    {
        _dbContext = dbContext;
        _searchTermRule = searchTermRule;
        _facetCalculator = facetCalculator;
        _searchResultsFromContextMapper = searchResultsFromContextMapper;
        _searchFilterRequestMapper = searchFilterRequestMapper;
        _searchFilterExpressionsBuilder = searchFilterExpressionsBuilder;
    }

    //
    //  This needs decomposing more for testability - maybe look at creating a pipeline (of strategies)
    //  to faciltate granular unit testing whilst also allowing for extensibility via composition.
    //
    public async Task<SearchResults<EstablishmentSearchResults, SearchFacets>> SearchAsync(
        SearchServiceAdapterRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Establishment> baseQuery = _dbContext.Establishment.AsNoTracking();
        IQueryable<Establishment> filteredQuery = baseQuery;

        ReadOnlyCollection<SearchFilterRequest> filterRequests =
            _searchFilterRequestMapper.Map(request.SearchFilterRequests.AsReadOnly());

        Expression<Func<Establishment, bool>> candidateFilter =
            _searchFilterExpressionsBuilder.BuildSearchFilterExpression(filterRequests);

        bool hasTerms = request.SearchTerms != null && request.SearchTerms.Any(st => st != null && !string.IsNullOrWhiteSpace(st.Value));

        if (hasTerms)
        {
            bool exactHandled = false;

            // 1. Delegated exact search across key query builders
            IQueryable<Establishment> exactQuery = _searchTermRule.ApplyExactSearch(
                baseQuery,
                request.SearchTerms,
                out bool hasExactMatchers);

            if (hasExactMatchers)
            {
                IQueryable<Establishment> candidateFiltered = exactQuery.Where(candidateFilter);

                int exactCount = await candidateFiltered.CountAsync(cancellationToken);
                if (exactCount > 0)
                {
                    filteredQuery = candidateFiltered;
                    exactHandled = true;
                }
            }

            // 2. Broad search fallback across key query builders
            if (!exactHandled)
            {
                IQueryable<Establishment> searchedQuery = _searchTermRule.ApplySearch(baseQuery, request.SearchTerms);
                filteredQuery = searchedQuery.Where(candidateFilter);
            }
        }
        else
        {
            filteredQuery = baseQuery.Where(candidateFilter);
        }

        int totalCount = await filteredQuery.CountAsync(cancellationToken);

        List<EstablishmentReadModel> items = await filteredQuery
            .Skip(request.Offset)
            .Take(request.PageSize)
            .Select(e => new EstablishmentReadModel(
                int.Parse(e.EstablishmentId.ToString()),
                e.Urn,
                e.Uid,
                e.Name,
                e.Site.Select(s => s.Postcode).FirstOrDefault(),
                e.Site.Select(s => s.County).FirstOrDefault(),
                e.EstablishmentType != null ? e.EstablishmentType.Name : string.Empty,
                e.EstablishmentStatus != null ? e.EstablishmentStatus.Name : string.Empty))
            .ToListAsync(cancellationToken);

        IReadOnlyList<string> urns = items.Select(e => e.Urn).ToList().AsReadOnly();

        IReadOnlyList<AggregatedFacetResult> facets = await _facetCalculator.CalculateFacetsAsync(
            urns,
            request.Facets,
            cancellationToken);

        return _searchResultsFromContextMapper.Map((items, facets));
    }
}


# region Query Builder Work - To be refactored towards core library concerns

// This sets the blueprint for a specific usecase/approach, but we'll look to delegate
// this underlying behaviour to a core/reusable/agnostic projection AST engine. 
//
public interface ISearchColumnQueryBuilder
{
    string Key { get; }
    Expression<Func<Establishment, bool>> BuildExpression(string value);
    Expression<Func<Establishment, bool>>? BuildExactExpression(string value) => null;
}

// --- "What" Column Query Builders ---
//
//  We know we need to improve this somewhat by not grouping via specific
//  keys (i.e. the atomic builder has to have implicit knowledge of wider
//  grouping concerns) BUT it's felt this is something we can move towards
//  iteratively and will be handled ultimately via the query engine via
//  structured rules which can be established on a case by case basis.
//
public class UrnColumnQueryBuilder : ISearchColumnQueryBuilder
{
    public string Key => "what";

    public Expression<Func<Establishment, bool>> BuildExpression(string value) =>
        e => e.Urn == value || e.Urn.Contains(value);

    public Expression<Func<Establishment, bool>>? BuildExactExpression(string value) =>
        e => e.Urn == value;
}

public class UidColumnQueryBuilder : ISearchColumnQueryBuilder
{
    public string Key => "what";

    public Expression<Func<Establishment, bool>> BuildExpression(string value) =>
        e => e.Uid == value || e.Uid.Contains(value);

    public Expression<Func<Establishment, bool>>? BuildExactExpression(string value) =>
        e => e.Uid == value;
}

public class NameColumnQueryBuilder : ISearchColumnQueryBuilder
{
    public string Key => "what";
    private const double WordSimilarityThreshold = 0.4;

    public Expression<Func<Establishment, bool>> BuildExpression(string value) =>
        e => e.Name.ToLower() == value.ToLower() ||
             EF.Functions.ILike(e.Name, $"%{value}%") ||
             EF.Functions.TrigramsWordSimilarity(value, e.Name) >= WordSimilarityThreshold;

    public Expression<Func<Establishment, bool>>? BuildExactExpression(string value) =>
        e => e.Name.ToLower() == value.ToLower();
}

// --- "Where" Column Query Builders ---
//
//  We know we need to improve this somewhat by not grouping via specific
//  keys (i.e. the atomic builder has to have implicit knowledge of wider
//  grouping concerns) BUT it's felt this is something we can move towards
//  iteratively and will be handled ultimately via the query engine via
//  structured rules which can be established on a case by case basis.
//
public class PostcodeColumnQueryBuilder : ISearchColumnQueryBuilder
{
    public string Key => "where";

    public Expression<Func<Establishment, bool>> BuildExpression(string value) =>
        e => e.Site.Any(s => s.Postcode != null && EF.Functions.ILike(s.Postcode, $"%{value}%"));
}

public class CountyColumnQueryBuilder : ISearchColumnQueryBuilder
{
    public string Key => "where";

    public Expression<Func<Establishment, bool>> BuildExpression(string value) =>
        e => e.Site.Any(s => s.County != null && EF.Functions.ILike(s.County, $"%{value}%"));
}

public class CityColumnQueryBuilder : ISearchColumnQueryBuilder
{
    public string Key => "where";

    public Expression<Func<Establishment, bool>> BuildExpression(string value) =>
        e => e.Site.Any(s => s.Town != null && EF.Functions.ILike(s.Town, $"%{value}%"));
}

//
//  This is just composotion which we'll ultimately handle via the query engine.
//
public interface ISearchKeyQueryBuilder<TEntity>
{
    string Key { get; }
    Expression<Func<TEntity, bool>> BuildExpression(string value);
    Expression<Func<TEntity, bool>>? BuildExactExpression(string value);
}

public class CompositeSearchKeyQueryBuilder : ISearchKeyQueryBuilder<Establishment>
{
    private readonly List<ISearchColumnQueryBuilder> _columnBuilders;

    public string Key { get; }

    public CompositeSearchKeyQueryBuilder(string key, IEnumerable<ISearchColumnQueryBuilder> columnBuilders)
    {
        Key = key.ToLowerInvariant();
        _columnBuilders = columnBuilders
            .Where(b => b.Key.Equals(Key, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public Expression<Func<Establishment, bool>> BuildExpression(string value)
    {
        string term = value.Trim();
        Expression<Func<Establishment, bool>>? combined = null;

        foreach (ISearchColumnQueryBuilder builder in _columnBuilders)
        {
            // We know we have baked in predicate here which needs to be configurable,
            // again, the query engine will be designed to handle this more elegantly
            // in a configurable manner.
            Expression<Func<Establishment, bool>> expr = builder.BuildExpression(term);
            combined = combined == null ? expr : combined.Or(expr);
        }

        return combined!;
    }

    public Expression<Func<Establishment, bool>>? BuildExactExpression(string value)
    {
        string term = value.Trim();
        Expression<Func<Establishment, bool>>? combined = null;

        foreach (ISearchColumnQueryBuilder builder in _columnBuilders)
        {
            Expression<Func<Establishment, bool>>? exactExpr = builder.BuildExactExpression(term);
            if (exactExpr != null)
            {
                combined = combined == null ? exactExpr : combined.Or(exactExpr);
            }
        }

        return combined;
    }
}




public interface ISearchTermRule<TQuery>
{
    TQuery ApplySearch(TQuery query, IEnumerable<SearchTerm?>? searchTerms);
    TQuery ApplyExactSearch(TQuery query, IEnumerable<SearchTerm?>? searchTerms, out bool hasExactMatchers);
}


//
//  This part deals with search term expression composition whereby the build-up of
//  specific query defintions, i.e. which field(s) to query over, and how to query (i.e. exact, partial, etc).
//  
//  This WILL be promoted to the query engine, and it SHOULD establish these sort of defintions based on the
//  incoming search request (term key), cosolidated against some prescription (i.e. this key is associated
//  with this configured set of fields and associated query behaviour) which the engine will compose and deliver.
//  The engine will be fully agnostic to a given service, i.e. rules can be configured and prescribed on a
//  case-by-case basis.
//
public class ComposableSearchTermRule : ISearchTermRule<IQueryable<Establishment>>
{
    private readonly IDictionary<string, ISearchKeyQueryBuilder<Establishment>> _keyBuilders;

    public ComposableSearchTermRule(IEnumerable<ISearchKeyQueryBuilder<Establishment>> keyBuilders)
    {
        _keyBuilders = keyBuilders.ToDictionary(b => b.Key.ToLowerInvariant(), b => b, StringComparer.OrdinalIgnoreCase);
    }

    public IQueryable<Establishment> ApplySearch(IQueryable<Establishment> query, IEnumerable<SearchTerm?>? searchTerms)
    {
        List<SearchTerm> validTerms = GetValidSearchTerms(searchTerms);
        if (validTerms.Count == 0)
            return query;

        Expression<Func<Establishment, bool>>? totalExpression = null;

        foreach (SearchTerm term in validTerms)
        {
            if (_keyBuilders.TryGetValue(term.Key, out ISearchKeyQueryBuilder<Establishment>? keyBuilder))
            {
                Expression<Func<Establishment, bool>> expr = keyBuilder.BuildExpression(term.Value);
                if (expr != null)
                {
                    // We know we have baked in predicate here which needs to be configurable,
                    // again, the query engine will be designed to handle this more elegantly
                    // in a configurable manner.
                    totalExpression = totalExpression == null ? expr : totalExpression.And(expr);
                }
            }
        }

        return totalExpression != null ? query.Where(totalExpression) : query;
    }

    public IQueryable<Establishment> ApplyExactSearch(
        IQueryable<Establishment> query,
        IEnumerable<SearchTerm?>? searchTerms,
        out bool hasExactMatchers)
    {
        hasExactMatchers = false;
        List<SearchTerm> validTerms = GetValidSearchTerms(searchTerms);
        if (validTerms.Count == 0)
            return query;

        Expression<Func<Establishment, bool>>? totalExpression = null;

        foreach (SearchTerm term in validTerms)
        {
            if (_keyBuilders.TryGetValue(term.Key, out ISearchKeyQueryBuilder<Establishment>? keyBuilder))
            {
                Expression<Func<Establishment, bool>>? exactExpr = keyBuilder.BuildExactExpression(term.Value);

                if (exactExpr != null)
                {
                    hasExactMatchers = true;
                    totalExpression = totalExpression == null ? exactExpr : totalExpression.And(exactExpr);
                }
                else
                {
                    // Fall back to broad expression for keys without explicit exact logic (e.g. "where")
                    Expression<Func<Establishment, bool>> broadExpr = keyBuilder.BuildExpression(term.Value);
                    if (broadExpr != null)
                    {
                        // We know we have baked in predicate here which needs to be configurable,
                        // again, the query engine will be designed to handle this more elegantly
                        // in a configurable manner.
                        totalExpression = totalExpression == null ? broadExpr : totalExpression.And(broadExpr);
                    }
                }
            }
        }

        return (hasExactMatchers && totalExpression != null) ? query.Where(totalExpression) : query;
    }

    private static List<SearchTerm> GetValidSearchTerms(IEnumerable<SearchTerm?>? searchTerms) =>
        searchTerms?
            .Where(t => t is not null && !string.IsNullOrWhiteSpace(t.Key) && !string.IsNullOrWhiteSpace(t.Value))
            .Select(t => t!)
            .ToList() ?? [];
}

//
// Machinery to compose and combine expressions with specific predicates (Or/And) again
// to be handled in a more agnostic way via query engine.
//
public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T));
        Expression? left = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter).Visit(expr1.Body);
        Expression? right = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter).Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left!, right!), parameter);
    }

    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T));
        Expression? left = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter).Visit(expr1.Body);
        Expression? right = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter).Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left!, right!), parameter);
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node) => node == _oldValue ? _newValue : base.Visit(node);
    }
}

#endregion

public record AggregatedFacetResult(string FacetName, IReadOnlyCollection<FacetResult> Values);

public record EstablishmentReadModel(
    int Id,
    string Urn,
    string Ukprn,
    string Name,
    string? Postcode,
    string? City,
    string Type,
    string Status
);

#region Promoted to core infra

//
//  This is an Infra concern and could become part of it's own reusable piece
//  BUT not a concern of the query engine (it's job is to surface a configured
//  Query AST and provide options for translation into usable SQL.
//
public interface IFacetAggregator
{
    Task<IReadOnlyList<AggregatedFacetResult>> CalculateFacetsAsync(
        IReadOnlyList<string> urns,
        IEnumerable<string>? requestedFacets,
        CancellationToken cancellationToken);
}

public class FacetAggregationStep : IFacetAggregator
{
    private readonly IFacetProvider _facetProvider;

    public FacetAggregationStep(IFacetProvider facetProvider)
    {
        _facetProvider = facetProvider;
    }

    public async Task<IReadOnlyList<AggregatedFacetResult>> CalculateFacetsAsync(
        IReadOnlyList<string> urns,
        IEnumerable<string>? requestedFacets,
        CancellationToken cancellationToken)
    {
        if (requestedFacets == null || !requestedFacets.Any())
        {
            return Array.Empty<AggregatedFacetResult>();
        }

        List<AggregatedFacetResult> aggregatedFacetResults = [];

        foreach (string facetKey in requestedFacets.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            IReadOnlyList<FacetResult> facetResults =
                await _facetProvider.GetFacetsAsync(urns, facetKey, cancellationToken);

            aggregatedFacetResults.Add(new AggregatedFacetResult(facetKey, facetResults));
        }

        return aggregatedFacetResults.AsReadOnly();
    }
}

#endregion
