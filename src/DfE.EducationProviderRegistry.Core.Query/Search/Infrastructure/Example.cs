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

public record SearchTerm(string Key, string Value);

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
    IReadOnlyCollection<SearchTerm>? SearchTerms = null,
    int Offset = 0,
    int PageSize = 20,
    List<FilterCriterion>? Filters = null,
    List<string>? RequestedFacets = null,
    SortByOption SortBy = SortByOption.NameAsc
);

public record EstablishmentSearchResponse(
    IReadOnlyCollection<EstablishmentReadModel> Items,
    int TotalCount,
    IReadOnlyCollection<FacetResultNew> Facets,
    int Offset,
    int PageSize
);

/// <summary>
/// Builds a LINQ expression tree for a single database column or entity property.
/// </summary>
public interface ISearchColumnQueryBuilder
{
    /// <summary>
    /// The contract key this column maps to (e.g., "what", "where", "who").
    /// </summary>
    string Key { get; }

    Expression<Func<Establishment, bool>> BuildExpression(string value);

    Expression<Func<Establishment, bool>>? BuildExactExpression(string value) => null;
}

// "What" Column Query Builders
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

// "Where" Column Query Builders
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

/// <summary>
/// Composes expression builders for a specific contract key (e.g. "what", "where").
/// </summary>
public interface ISearchKeyQueryBuilder<TEntity>
{
    string Key { get; }
    Expression<Func<TEntity, bool>> BuildExpression(string value);
    Expression<Func<TEntity, bool>>? BuildExactExpression(string value);
}

/// <summary>
/// Dynamically composes all ISearchColumnQueryBuilder instances registered for a specific Key using OR logic.
/// </summary>
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
    TQuery ApplySearch(TQuery query, IEnumerable<SearchTerm>? searchTerms);
    TQuery ApplyExactSearch(TQuery query, IEnumerable<SearchTerm>? searchTerms, out bool hasExactMatchers);
}

public class ComposableSearchTermRule : ISearchTermRule<IQueryable<Establishment>>
{
    private readonly IDictionary<string, ISearchKeyQueryBuilder<Establishment>> _keyBuilders;

    public ComposableSearchTermRule(IEnumerable<ISearchKeyQueryBuilder<Establishment>> keyBuilders)
    {
        _keyBuilders = keyBuilders.ToDictionary(b => b.Key.ToLowerInvariant(), b => b);
    }

    public IQueryable<Establishment> ApplySearch(IQueryable<Establishment> query, IEnumerable<SearchTerm>? searchTerms)
    {
        List<SearchTerm> validTerms = GetValidSearchTerms(searchTerms);
        if (!validTerms.Any())
            return query;

        Expression<Func<Establishment, bool>>? totalExpression = null;

        foreach (SearchTerm term in validTerms)
        {
            string key = term.Key.ToLowerInvariant();

            if (_keyBuilders.TryGetValue(key, out ISearchKeyQueryBuilder<Establishment>? keyBuilder))
            {
                Expression<Func<Establishment, bool>> expr = keyBuilder.BuildExpression(term.Value);
                if (expr != null)
                {
                    totalExpression = totalExpression == null ? expr : totalExpression.And(expr);
                }
            }
            else
            {
                throw new ArgumentException($"Unsupported search key: '{term.Key}'. No query builder registered.");
            }
        }

        return totalExpression != null ? query.Where(totalExpression) : query;
    }

    public IQueryable<Establishment> ApplyExactSearch(
        IQueryable<Establishment> query,
        IEnumerable<SearchTerm>? searchTerms,
        out bool hasExactMatchers)
    {
        hasExactMatchers = false;
        List<SearchTerm> validTerms = GetValidSearchTerms(searchTerms);
        if (!validTerms.Any())
            return query;

        Expression<Func<Establishment, bool>>? totalExpression = null;

        foreach (SearchTerm term in validTerms)
        {
            string key = term.Key.ToLowerInvariant();

            if (_keyBuilders.TryGetValue(key, out ISearchKeyQueryBuilder<Establishment>? keyBuilder))
            {
                Expression<Func<Establishment, bool>>? exactExpr = keyBuilder.BuildExactExpression(term.Value);

                if (exactExpr != null)
                {
                    hasExactMatchers = true;
                    totalExpression = totalExpression == null ? exactExpr : totalExpression.And(exactExpr);
                }
                else
                {
                    // Fall back to broad expression for key builders without exact match logic (e.g. "where")
                    Expression<Func<Establishment, bool>> broadExpr = keyBuilder.BuildExpression(term.Value);
                    if (broadExpr != null)
                    {
                        totalExpression = totalExpression == null ? broadExpr : totalExpression.And(broadExpr);
                    }
                }
            }
            else
            {
                throw new ArgumentException($"Unsupported search key: '{term.Key}'. No query builder registered.");
            }
        }

        return (hasExactMatchers && totalExpression != null) ? query.Where(totalExpression) : query;
    }

    private static List<SearchTerm> GetValidSearchTerms(IEnumerable<SearchTerm>? searchTerms) =>
        searchTerms?
            .Where(t => !string.IsNullOrWhiteSpace(t.Key) && !string.IsNullOrWhiteSpace(t.Value))
            .ToList() ?? new List<SearchTerm>();
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

public interface IFilterRule<TQuery>
{
    string FieldKey { get; }
    TQuery Apply(TQuery query, IEnumerable<string> values);
}

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

public interface ISortStrategy<TQuery>
{
    TQuery ApplySorting(TQuery query, SortByOption sortBy, string? searchTerm);
}

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


public interface IFacetProvider<TQuery>
{
    string FacetKey { get; }
    Task<FacetResultNew> BuildFacetAsync(TQuery query, CancellationToken cancellationToken);
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

public interface IFacetCalculator<TQuery>
{
    Task<List<FacetResultNew>> CalculateFacetsAsync(
        TQuery query,
        IEnumerable<string>? requestedFacets,
        CancellationToken cancellationToken);
}

public class ConfigurableEfFacetCalculator : IFacetCalculator<IQueryable<Establishment>>
{
    private readonly IDictionary<string, IFacetProvider<IQueryable<Establishment>>> _facetProviders;

    public ConfigurableEfFacetCalculator(IEnumerable<IFacetProvider<IQueryable<Establishment>>> facetProviders)
    {
        _facetProviders = facetProviders.ToDictionary(p => p.FacetKey.ToLowerInvariant());
    }

    public async Task<List<FacetResultNew>> CalculateFacetsAsync(
        IQueryable<Establishment> query,
        IEnumerable<string>? requestedFacets,
        CancellationToken cancellationToken)
    {
        if (requestedFacets == null || !requestedFacets.Any())
            return new List<FacetResultNew>();

        List<FacetResultNew> results = new List<FacetResultNew>();

        foreach (string? facetKey in requestedFacets.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (_facetProviders.TryGetValue(facetKey.ToLowerInvariant(), out IFacetProvider<IQueryable<Establishment>>? provider))
            {
                results.Add(await provider.BuildFacetAsync(query, cancellationToken));
            }
        }

        return results;
    }
}


public interface IEstablishmentSearchAdapter
{
    Task<EstablishmentSearchResponse> SearchAsync(
        EstablishmentSearchRequest request,
        CancellationToken cancellationToken = default);
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
        IQueryable<Establishment> baseQuery = _dbContext.Establishment.AsNoTracking();
        IQueryable<Establishment> filteredQuery = baseQuery;

        bool hasTerms = request.SearchTerms != null && request.SearchTerms.Any(t => !string.IsNullOrWhiteSpace(t.Value));

        if (hasTerms)
        {
            bool exactHandled = false;

            // 1. Try exact identity match across provided terms via registered key builders
            IQueryable<Establishment> exactQuery = _searchTermRule.ApplyExactSearch(baseQuery, request.SearchTerms, out bool hasExactMatchers);

            if (hasExactMatchers)
            {
                IQueryable<Establishment> candidateFiltered = ApplyFilters(exactQuery, request.Filters);
                int exactCount = await candidateFiltered.CountAsync(cancellationToken);

                if (exactCount > 0)
                {
                    filteredQuery = candidateFiltered;
                    exactHandled = true;
                }
            }

            // 2. Fall back to broad search via key builders
            if (!exactHandled)
            {
                IQueryable<Establishment> searchedQuery = _searchTermRule.ApplySearch(baseQuery, request.SearchTerms);
                filteredQuery = ApplyFilters(searchedQuery, request.Filters);
            }
        }
        else
        {
            filteredQuery = ApplyFilters(baseQuery, request.Filters);
        }

        int totalCount = await filteredQuery.CountAsync(cancellationToken);

        string? primaryTerm = GetPrimarySearchTerm(request.SearchTerms);

        IQueryable<Establishment> sortedQuery = _sortStrategy.ApplySorting(filteredQuery, request.SortBy, primaryTerm);

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
            filteredQuery,
            request.RequestedFacets,
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

    private static string? GetPrimarySearchTerm(IEnumerable<SearchTerm>? searchTerms)
    {
        if (searchTerms == null)
            return null;
        SearchTerm? whatTerm = searchTerms.FirstOrDefault(t => t.Key.Equals("what", StringComparison.OrdinalIgnoreCase));

        if (whatTerm != null && !string.IsNullOrWhiteSpace(whatTerm.Value))
            return whatTerm.Value;

        return searchTerms.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.Value))?.Value;
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
                    throw new ArgumentException($"Unsupported search filter field: '{filter.Field}'");
                }
            }
        }

        return query;
    }
}
