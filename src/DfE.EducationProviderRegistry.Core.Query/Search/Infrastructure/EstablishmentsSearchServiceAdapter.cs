using System.Collections.ObjectModel;
using System.Data;
using System.Linq.Expressions;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
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

    public async Task<SearchResults<EstablishmentSearchResults, SearchFacets>> SearchAsync(
    SearchServiceAdapterRequest request,
    CancellationToken cancellationToken = default)
    {
        IQueryable<Establishment> baseQuery = _dbContext.Establishment.AsNoTracking();
        IQueryable<Establishment> filteredQuery = baseQuery;

        bool hasWhat = !string.IsNullOrWhiteSpace(request.WhatTerm);
        bool hasWhere = !string.IsNullOrWhiteSpace(request.WhereTerm);

        ReadOnlyCollection<SearchFilterRequest> filterRequests =
            _searchFilterRequestMapper
                .Map(request.SearchFilterRequests.AsReadOnly());

        if (hasWhat || hasWhere)
        {
            bool exactHandled = false;

            // 1. Check Exact Identity Match on WhatTerm if provided
            if (hasWhat)
            {
                string term = request.WhatTerm!.Trim();

                IQueryable<Establishment> exactWhatQuery = baseQuery.Where(e =>
                    e.Urn == term ||
                    e.Uid == term ||
                    e.Name.ToLower() == term.ToLower());

                // Enforce location criteria on the exact match if WhereTerm is also passed
                if (hasWhere)
                {
                    exactWhatQuery = _searchTermRule.ApplyWhereOnly(exactWhatQuery, request.WhereTerm!);
                }

                Expression<Func<Establishment, bool>> candidateFilter =
                    _searchFilterExpressionsBuilder
                        .BuildSearchFilterExpression(filterRequests);

                IQueryable<Establishment> candidateFiltered = exactWhatQuery.Where(candidateFilter);

                int exactCount = await candidateFiltered.CountAsync(cancellationToken);

                if (exactCount > 0)
                {
                    filteredQuery = candidateFiltered;
                    exactHandled = true;
                }
            }

            // 2. Broad Search Fallback (if no exact match or if only WhereTerm was supplied)
            if (!exactHandled)
            {
                IQueryable<Establishment> searchedQuery =
                    _searchTermRule.ApplySearch(baseQuery, request.WhatTerm, request.WhereTerm);

                Expression<Func<Establishment, bool>> candidateFilter =
                    _searchFilterExpressionsBuilder.BuildSearchFilterExpression(filterRequests);

                filteredQuery = searchedQuery.Where(candidateFilter);
            }
        }
        else
        {
            Expression<Func<Establishment, bool>> candidateFilter =
                _searchFilterExpressionsBuilder
                    .BuildSearchFilterExpression(filterRequests);

            filteredQuery = baseQuery.Where(candidateFilter);
        }

        int totalCount = await filteredQuery.CountAsync(cancellationToken);

        List<EstablishmentReadModel> items =
            await filteredQuery
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

        IReadOnlyList<string> urns =
            items.Select(establishment =>
                establishment.Urn)
                    .ToList()
                    .AsReadOnly();

        IReadOnlyList<AggregatedFacetResult> facets =
            await _facetCalculator.CalculateFacetsAsync(
                urns,
                request.Facets,
                cancellationToken
        );

        return _searchResultsFromContextMapper.Map((items, facets));
    }
}

public enum SearchFieldCategory
{
    What,
    Where
}

public record AggregatedFacetResult(
    string FacetName, IReadOnlyCollection<FacetResult> Values);

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

public interface ISearchTermRule<TQuery>
{
    TQuery ApplySearch(TQuery query, string? whatTerm, string? whereTerm);
    TQuery ApplyWhereOnly(TQuery query, string whereTerm);
}

public interface ISearchFieldMatcher
{
    SearchFieldCategory Category { get; }
    Expression<Func<Establishment, bool>> GetExpression(string searchTerm);
}

#region This is how it currently determines what fields to search across

// "What" Matchers (Name, URN, UID)
public class UrnSearchMatcher : ISearchFieldMatcher
{
    public SearchFieldCategory Category => SearchFieldCategory.What;

    public Expression<Func<Establishment, bool>> GetExpression(string searchTerm) =>
        e => e.Urn == searchTerm || e.Urn.Contains(searchTerm);
}

public class UidSearchMatcher : ISearchFieldMatcher
{
    public SearchFieldCategory Category => SearchFieldCategory.What;

    public Expression<Func<Establishment, bool>> GetExpression(string searchTerm) =>
        e => e.Uid == searchTerm || e.Uid.Contains(searchTerm);
}

public class NameSearchMatcher : ISearchFieldMatcher
{
    public SearchFieldCategory Category => SearchFieldCategory.What;
    private const double WordSimilarityThreshold = 0.4;

    public Expression<Func<Establishment, bool>> GetExpression(string searchTerm) =>
        e => e.Name.ToLower() == searchTerm.ToLower() ||
             EF.Functions.ILike(e.Name, $"%{searchTerm}%") ||
             EF.Functions.TrigramsWordSimilarity(searchTerm, e.Name) >= WordSimilarityThreshold;
}

public class PostcodeSearchMatcher : ISearchFieldMatcher
{
    public SearchFieldCategory Category => SearchFieldCategory.Where;

    public Expression<Func<Establishment, bool>> GetExpression(string searchTerm) =>
        e => e.Site.Any(s => s.Postcode != null && EF.Functions.ILike(s.Postcode, $"%{searchTerm}%"));
}

public class CountySearchMatcher : ISearchFieldMatcher
{
    public SearchFieldCategory Category => SearchFieldCategory.Where;

    public Expression<Func<Establishment, bool>> GetExpression(string searchTerm) =>
        e => e.Site.Any(s => s.County != null && EF.Functions.ILike(s.County, $"%{searchTerm}%"));
}

public class CitySearchMatcher : ISearchFieldMatcher
{
    public SearchFieldCategory Category => SearchFieldCategory.Where;

    public Expression<Func<Establishment, bool>> GetExpression(string searchTerm) =>
        e => e.Site.Any(s => s.Town != null && EF.Functions.ILike(s.Town, $"%{searchTerm}%"));
}

public class ComposableSearchTermRule : ISearchTermRule<IQueryable<Establishment>>
{
    private readonly IEnumerable<ISearchFieldMatcher> _matchers;

    public ComposableSearchTermRule(IEnumerable<ISearchFieldMatcher> matchers)
    {
        _matchers = matchers;
    }

    public IQueryable<Establishment> ApplySearch(IQueryable<Establishment> query, string? whatTerm, string? whereTerm)
    {
        Expression<Func<Establishment, bool>>? whatExpr = BuildCategoryExpression(SearchFieldCategory.What, whatTerm);
        Expression<Func<Establishment, bool>>? whereExpr = BuildCategoryExpression(SearchFieldCategory.Where, whereTerm);

        if (whatExpr is not null && whereExpr is not null)
        {
            return query.Where(whatExpr.And(whereExpr));
        }

        if (whatExpr is not null)
            return query.Where(whatExpr);
        if (whereExpr is not null)
            return query.Where(whereExpr);

        return query;
    }

    public IQueryable<Establishment> ApplyWhereOnly(IQueryable<Establishment> query, string whereTerm)
    {
        Expression<Func<Establishment, bool>>? whereExpr = BuildCategoryExpression(SearchFieldCategory.Where, whereTerm);
        return whereExpr != null ? query.Where(whereExpr) : query;
    }

    private Expression<Func<Establishment, bool>>? BuildCategoryExpression(SearchFieldCategory category, string? term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return null;

        string cleanTerm = term.Trim();
        List<ISearchFieldMatcher> categoryMatchers = _matchers.Where(m => m.Category == category).ToList();

        Expression<Func<Establishment, bool>>? combined = null;
        foreach (ISearchFieldMatcher matcher in categoryMatchers)
        {
            Expression<Func<Establishment, bool>> expr = matcher.GetExpression(cleanTerm);
            combined = combined == null ? expr : combined.Or(expr);
        }

        return combined;
    }
}

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

            aggregatedFacetResults.AddRange(
                new AggregatedFacetResult(facetKey, facetResults));
        }

        return aggregatedFacetResults.AsReadOnly();
    }

}
