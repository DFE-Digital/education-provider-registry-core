using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;


public enum SortByOption
{
    NameAsc,
    NameDesc,
    UrnAsc,
    UrnDesc,
    Relevance
}

public enum SearchFieldCategory
{
    What,
    Where
}

// Models
public record FilterCriterion(string Field, List<string> Values);
public record FacetValueResult(string Value, int Count);
public record FacetResultNew(string FacetName, IReadOnlyCollection<FacetValueResult> Values);

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

public record EstablishmentSearchRequest(
    string? WhatTerm = null,
    string? WhereTerm = null,
    int Offset = 0,
    int PageSize = 20,
    List<FilterCriterion>? Filters = null,
    List<string>? RequestedFacets = null,
    SortByOption SortBy = SortByOption.NameAsc,
    bool ScopeFacetsToFilters = false
);

public record EstablishmentSearchResponse(
    IReadOnlyCollection<EstablishmentReadModel> Items,
    int TotalCount,
    IReadOnlyCollection<FacetResultNew> Facets,
    int Offset,
    int PageSize
);

// Base abstractions
public interface IEstablishmentSearchAdapter
{
    Task<EstablishmentSearchResponse> SearchAsync(
        EstablishmentSearchRequest request,
        CancellationToken cancellationToken = default);
}

public interface ISearchTermRule<TQuery>
{
    TQuery ApplySearch(TQuery query, string? whatTerm, string? whereTerm);
    TQuery ApplyWhereOnly(TQuery query, string whereTerm);
}

public interface IFilterRule<TQuery>
{
    string FieldKey { get; }
    TQuery Apply(TQuery query, IEnumerable<string> values);
}

public interface ISortStrategy<TQuery>
{
    TQuery ApplySorting(TQuery query, SortByOption sortBy, string? searchTerm);
}

public interface IFacetProvider<TQuery>
{
    string FacetKey { get; }
    Task<FacetResultNew> BuildFacetAsync(TQuery query, CancellationToken cancellationToken);
}

public interface IFacetCalculator<TQuery>
{
    Task<List<FacetResultNew>> CalculateFacetsAsync(
        TQuery baseMatchQuery,
        TQuery fullyFilteredQuery,
        IEnumerable<string>? requestedFacets,
        bool scopeToFilters,
        CancellationToken cancellationToken);
}

public interface ISearchFieldMatcher
{
    SearchFieldCategory Category { get; }
    Expression<Func<Establishment, bool>> GetExpression(string searchTerm);
}


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

// "Where" Matchers (Postcode, County, City)

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

// Composable search
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
            // Both populated: (What_1 OR What_2) AND (Where_1 OR Where_2)
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

// Expressions
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


// Filter rules
public class PostcodeFilterRule : IFilterRule<IQueryable<Establishment>>
{
    public string FieldKey => "postcode";

    public IQueryable<Establishment> Apply(IQueryable<Establishment> query, IEnumerable<string> values) =>
        query.Where(e => e.Site.Any(s => s.Postcode != null && values.Any(v => s.Postcode.Contains(v))));
}

public class UidFilterRule : IFilterRule<IQueryable<Establishment>>
{
    public string FieldKey => "uid";

    public IQueryable<Establishment> Apply(IQueryable<Establishment> query, IEnumerable<string> values) =>
        query.Where(e => values.Contains(e.Uid));
}

public class CountyFilterRule : IFilterRule<IQueryable<Establishment>>
{
    public string FieldKey => "county";

    public IQueryable<Establishment> Apply(IQueryable<Establishment> query, IEnumerable<string> values) =>
        query.Where(e => e.Site.Any(s => s.County != null && values.Contains(s.County)));
}

public class TypeFilterRule : IFilterRule<IQueryable<Establishment>>
{
    public string FieldKey => "type";

    public IQueryable<Establishment> Apply(IQueryable<Establishment> query, IEnumerable<string> values)
    {
        List<string>? valueList = values?.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();

        if (valueList is null || !valueList.Any())
            return query;

        return query.Where(e => e.EstablishmentType != null && valueList.Contains(e.EstablishmentType.Name));
    }
}

public class StatusFilterRule : IFilterRule<IQueryable<Establishment>>
{
    public string FieldKey => "status";

    public IQueryable<Establishment> Apply(IQueryable<Establishment> query, IEnumerable<string> values)
    {
        List<string>? valueList = values?.Where(v => !string.IsNullOrWhiteSpace(v)).ToList();

        if (valueList is null || !valueList.Any())
            return query;

        return query.Where(e => e.EstablishmentStatus != null && valueList.Contains(e.EstablishmentStatus.Name));
    }
}


// Sorting
public class EfSortStrategy : ISortStrategy<IQueryable<Establishment>>
{
    public IQueryable<Establishment> ApplySorting(
        IQueryable<Establishment> query,
        SortByOption sortBy,
        string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return sortBy switch
            {
                SortByOption.NameDesc => query.OrderByDescending(e => e.Name),
                SortByOption.UrnAsc => query.OrderBy(e => e.Urn),
                SortByOption.UrnDesc => query.OrderByDescending(e => e.Urn),
                _ => query.OrderBy(e => e.Name)
            };
        }

        string term = searchTerm.Trim();

        return sortBy switch
        {
            SortByOption.NameAsc => query.OrderBy(e => e.Name),
            SortByOption.NameDesc => query.OrderByDescending(e => e.Name),
            SortByOption.UrnAsc => query.OrderBy(e => e.Urn),
            SortByOption.UrnDesc => query.OrderByDescending(e => e.Urn),

            // RELEVANCE TIERING:
            // 1. Exact URN or UID match
            // 2. Exact Name match (case-insensitive)
            // 3. Name Starts-With match (e.g. "The Poplars" ranks #1 when searching "the poplar")
            // 4. Trigram Word Similarity score
            // 5. Alphabetical fallback
            SortByOption.Relevance => query
                .OrderByDescending(e => e.Urn == term || e.Uid == term)
                .ThenByDescending(e => e.Name.ToLower() == term.ToLower())
                .ThenByDescending(e => e.Name.ToLower().StartsWith(term.ToLower()))
                .ThenByDescending(e => EF.Functions.TrigramsWordSimilarity(term, e.Name))
                .ThenBy(e => e.Name),

            _ => query.OrderBy(e => e.Name)
        };
    }
}


public class CountyFacetProvider : IFacetProvider<IQueryable<Establishment>>
{
    public string FacetKey => "county";

    public async Task<FacetResultNew> BuildFacetAsync(IQueryable<Establishment> query, CancellationToken cancellationToken)
    {
        List<FacetValueResult> values = await query
            .Where(e => e.Site.Any(s => s.County != null))
            .GroupBy(e => e.Site.Select(s => s.County).FirstOrDefault()!)
            .Select(g => new FacetValueResult(g.Key, g.Count()))
            .ToListAsync(cancellationToken);

        return new FacetResultNew("County", values);
    }
}

public class TypeFacetProvider : IFacetProvider<IQueryable<Establishment>>
{
    public string FacetKey => "type";

    public async Task<FacetResultNew> BuildFacetAsync(IQueryable<Establishment> query, CancellationToken cancellationToken)
    {
        List<FacetValueResult> values = await query
            .Where(e => e.EstablishmentType != null)
            .GroupBy(e => e.EstablishmentType.Name)
            .Select(g => new FacetValueResult(g.Key, g.Count()))
            .ToListAsync(cancellationToken);

        return new FacetResultNew("Type", values);
    }
}

public class StatusFacetProvider : IFacetProvider<IQueryable<Establishment>>
{
    public string FacetKey => "status";

    public async Task<FacetResultNew> BuildFacetAsync(IQueryable<Establishment> query, CancellationToken cancellationToken)
    {
        List<FacetValueResult> values = await query
            .Where(e => e.EstablishmentStatus != null)
            .GroupBy(e => e.EstablishmentStatus.Name)
            .Select(g => new FacetValueResult(g.Key, g.Count()))
            .ToListAsync(cancellationToken);

        return new FacetResultNew("Status", values);
    }
}

public class ConfigurableEfFacetCalculator : IFacetCalculator<IQueryable<Establishment>>
{
    private readonly IDictionary<string, IFacetProvider<IQueryable<Establishment>>> _facetProviders;

    public ConfigurableEfFacetCalculator(IEnumerable<IFacetProvider<IQueryable<Establishment>>> facetProviders)
    {
        _facetProviders = facetProviders.ToDictionary(p => p.FacetKey.ToLowerInvariant());
    }

    public async Task<List<FacetResultNew>> CalculateFacetsAsync(
        IQueryable<Establishment> baseMatchQuery,
        IQueryable<Establishment> fullyFilteredQuery,
        IEnumerable<string>? requestedFacets,
        bool scopeToFilters,
        CancellationToken cancellationToken)
    {
        if (requestedFacets == null || !requestedFacets.Any())
            return new List<FacetResultNew>();

        IQueryable<Establishment> sourceQuery = scopeToFilters ? fullyFilteredQuery : baseMatchQuery;
        List<FacetResultNew> results = new List<FacetResultNew>();

        // Sequential execution prevents DbContext thread safety issues
        foreach (string? facetKey in requestedFacets.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (_facetProviders.TryGetValue(facetKey.ToLowerInvariant(), out IFacetProvider<IQueryable<Establishment>>? provider))
            {
                results.Add(await provider.BuildFacetAsync(sourceQuery, cancellationToken));
            }
        }

        return results;
    }
}


public class EfEstablishmentSearchAdapter : IEstablishmentSearchAdapter
{
    private readonly EducationProviderRegistryDbContext _dbContext;
    private readonly ISearchTermRule<IQueryable<Establishment>> _searchTermRule;
    private readonly IDictionary<string, IFilterRule<IQueryable<Establishment>>> _filterRules;
    private readonly ISortStrategy<IQueryable<Establishment>> _sortStrategy;
    private readonly IFacetCalculator<IQueryable<Establishment>> _facetCalculator;

    public EfEstablishmentSearchAdapter(
        EducationProviderRegistryDbContext dbContext,
        ISearchTermRule<IQueryable<Establishment>> searchTermRule,
        IEnumerable<IFilterRule<IQueryable<Establishment>>> filterRules,
        ISortStrategy<IQueryable<Establishment>> sortStrategy,
        IFacetCalculator<IQueryable<Establishment>> facetCalculator)
    {
        _dbContext = dbContext;
        _searchTermRule = searchTermRule;
        _filterRules = filterRules.ToDictionary(r => r.FieldKey.ToLowerInvariant());
        _sortStrategy = sortStrategy;
        _facetCalculator = facetCalculator;
    }

    public async Task<EstablishmentSearchResponse> SearchAsync(
        EstablishmentSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = _dbContext.Establishment.AsNoTracking();

        IQueryable<Establishment> searchedQuery = baseQuery;
        IQueryable<Establishment> filteredQuery = baseQuery;

        bool hasWhat = !string.IsNullOrWhiteSpace(request.WhatTerm);
        bool hasWhere = !string.IsNullOrWhiteSpace(request.WhereTerm);

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

                IQueryable<Establishment> candidateFiltered = ApplyFilters(exactWhatQuery, request.Filters);
                int exactCount = await candidateFiltered.CountAsync(cancellationToken);

                if (exactCount > 0)
                {
                    searchedQuery = exactWhatQuery;
                    filteredQuery = candidateFiltered;
                    exactHandled = true;
                }
            }

            // 2. Broad Search Fallback (if no exact match or if only WhereTerm was supplied)
            if (!exactHandled)
            {
                searchedQuery = _searchTermRule.ApplySearch(baseQuery, request.WhatTerm, request.WhereTerm);
                filteredQuery = ApplyFilters(searchedQuery, request.Filters);
            }
        }
        else
        {
            filteredQuery = ApplyFilters(baseQuery, request.Filters);
        }

        int totalCount = await filteredQuery.CountAsync(cancellationToken);

        // Sort using WhatTerm for relevance if available, otherwise fallback to name
        IQueryable<Establishment> sortedQuery = _sortStrategy.ApplySorting(
            filteredQuery, request.SortBy, request.WhatTerm ?? request.WhereTerm);

        List<EstablishmentReadModel> items = await sortedQuery
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

        List<FacetResultNew> facets = await _facetCalculator.CalculateFacetsAsync(
            searchedQuery,
            filteredQuery,
            request.RequestedFacets,
            request.ScopeFacetsToFilters,
            cancellationToken
        );

        return new EstablishmentSearchResponse(
            Items: items,
            TotalCount: totalCount,
            Facets: facets,
            Offset: request.Offset,
            PageSize: request.PageSize
        );
    }

    private IQueryable<Establishment> ApplyFilters(IQueryable<Establishment> query, List<FilterCriterion>? filters)
    {
        if (filters is null || !filters.Any())
            return query;

        foreach (FilterCriterion filter in filters)
        {
            if (filter.Values != null && filter.Values.Any(v => !string.IsNullOrWhiteSpace(v)))
            {
                string fieldKey = filter.Field.ToLowerInvariant();

                if (_filterRules.TryGetValue(fieldKey, out IFilterRule<IQueryable<Establishment>>? rule))
                {
                    query = rule.Apply(query, filter.Values);
                }
                else
                {
                    throw new ArgumentException($"Unsupported or unregistered search filter field: '{filter.Field}'");
                }
            }
        }

        return query;
    }
}
