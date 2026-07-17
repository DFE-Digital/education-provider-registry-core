using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

[Flags]
public enum MatchType
{
    None,
    Exact,
    StartsWith,
    Partial,
    Fuzzy,
    All = Exact | StartsWith | Partial | Fuzzy
}

public enum FilterMatchingStrategy
{
    MatchAnyValue, // Logical OR (e.g. Status In ('Open', 'Closed')
    MatchAllValues, // Logic AND (e.g. Opened AND LocalAuthority
    BetweenRange // Min/Max (e.g. ageRange >= 1 AND ageRange <=10) testing extensibility
}

public record FieldDefinition(
    string PropertyPath,
    MatchType AllowedSearchRules = MatchType.All,
    FilterMatchingStrategy FilterStrategy = FilterMatchingStrategy.MatchAnyValue);

public static class SearchRegistry
{
    public static readonly Dictionary<string, FieldDefinition> FieldMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "urn", new("Urn", MatchType.Exact | MatchType.StartsWith , FilterMatchingStrategy.MatchAnyValue) },
        { "name", new("Name", MatchType.All, FilterMatchingStrategy.MatchAnyValue) },
        { "status", new("EstablishmentStatus.Name", MatchType.All, FilterMatchingStrategy.MatchAnyValue) },
        { "type", new("EstablishmentType.Name", MatchType.All, FilterMatchingStrategy.MatchAnyValue) },
    };

    public static Expression GetPropertyPath(ParameterExpression parameter, string path)
    {
        Expression property = parameter;
        foreach (string member in path.Split('.'))
            property = Expression.PropertyOrField(property, member);

        return property;
    }
}

// FILTER STRATEGIES
public interface IFilterStrategy
{
    public Expression? BuildExpression(Expression propertyPath, IEnumerable<object> values);
}

public class MatchAnyFilterStategy : IFilterStrategy
{
    public Expression? BuildExpression(Expression propertyPath, IEnumerable<object> values)
    {
        Expression? fieldChain = null;
        foreach (var val in values)
        {
            var constant = Expression.Constant(Convert.ChangeType(val, propertyPath.Type), propertyPath.Type);
            var equality = Expression.Equal(propertyPath, constant);
            fieldChain = fieldChain is null ? equality : Expression.OrElse(fieldChain, equality);
        }

        return fieldChain;
    }
}

public class BetweenRangeFilterStrategy : IFilterStrategy
{
    public Expression? BuildExpression(Expression propertyPath, IEnumerable<object> values)
    {
        object? minVal = values.ElementAtOrDefault(0);
        object? maxVal = values.ElementAtOrDefault(1);

        Expression? minExp = null;
        Expression? maxExp = null;

        if (minVal is not null && !string.IsNullOrWhiteSpace(minVal.ToString()))
        {
            ConstantExpression minConstant = Expression.Constant(Convert.ChangeType(minVal, propertyPath.Type), propertyPath.Type);
            minExp = Expression.GreaterThanOrEqual(propertyPath, minConstant);
        }

        if (maxVal is not null && !string.IsNullOrWhiteSpace(maxVal.ToString()))
        {
            ConstantExpression maxConstant = Expression.Constant(Convert.ChangeType(maxVal, propertyPath.Type), propertyPath.Type);
            maxExp = Expression.LessThanOrEqual(propertyPath, maxConstant);
        }

        if (minExp is not null && maxExp is not null)
            return Expression.AndAlso(minExp, maxExp);

        return minExp ?? maxExp;
    }
}


// BUILDERS
public static class FilterExpressionBbuilder<TEntity> where TEntity : class
{
    private static readonly Dictionary<FilterMatchingStrategy, IFilterStrategy> StrategyMap = new()
    {
        { FilterMatchingStrategy.MatchAnyValue, new MatchAnyFilterStategy() },
        { FilterMatchingStrategy.BetweenRange, new BetweenRangeFilterStrategy() }
    };

    public static Expression<Func<TEntity, bool>> BuildFilters(IEnumerable<FilterRequest> filters)
    {
        if (filters is null || !filters.Any())
            return e => true;

        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");
        Expression? combinedAndWhere = null;

        foreach (FilterRequest filter in filters)
        {
            if (!SearchRegistry.FieldMap.TryGetValue(filter.FilterName, out FieldDefinition? fieldDefinition))
                continue;
            if (!StrategyMap.TryGetValue(fieldDefinition.FilterStrategy, out IFilterStrategy? strategy))
                continue;

            Expression propertyPath = SearchRegistry.GetPropertyPath(parameter, fieldDefinition.PropertyPath);
            Expression? fieldExpression = strategy.BuildExpression(propertyPath, filter.FilterValues);

            if (fieldExpression is not null)
                combinedAndWhere = combinedAndWhere is null ? fieldExpression : Expression.AndAlso(combinedAndWhere, fieldExpression);
        }
        return combinedAndWhere is null ? e => true : Expression.Lambda<Func<TEntity, bool>>(combinedAndWhere, parameter);
    }
}

public static class SearchExpressionBuilder<TEntity> where TEntity : class
{
    private static readonly MethodInfo ILikeMethod = typeof(NpgsqlDbFunctionsExtensions)
        .GetMethod(nameof(NpgsqlDbFunctionsExtensions.ILike), [typeof(DbFunctions), typeof(string), typeof(string)])!;

    private static readonly MethodInfo TrigramsAreSimilarMethod = typeof(NpgsqlTrigramsDbFunctionsExtensions)
        .GetMethod(nameof(NpgsqlTrigramsDbFunctionsExtensions.TrigramsAreSimilar), [typeof(DbFunctions), typeof(string), typeof(string)])!;

    public static (Expression<Func<TEntity, bool>> Predicate, Expression<Func<TEntity, int>> Score) Build(SearchServiceAdapterRequest request)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");
        if (string.IsNullOrWhiteSpace(request.SearchKeyword) || !request.SearchFields.Any())
            return (e => true, e => 0);

        ConstantExpression efFunctions = Expression.Constant(EF.Functions);
        ConstantExpression searchTerm = Expression.Constant(request.SearchKeyword);
        ConstantExpression startsWithTerm = Expression.Constant($"{request.SearchKeyword}%");
        ConstantExpression partialTerm = Expression.Constant($"%{request.SearchKeyword}%");

        Expression? combinedWhere = null;
        Expression currentScoreSum = Expression.Constant(0);

        foreach (var field in request.SearchFields)
        {
            if (!SearchRegistry.FieldMap.TryGetValue(field, out FieldDefinition? fieldDefinition) || fieldDefinition.AllowedSearchRules == MatchType.None)
                continue;

            Expression path = SearchRegistry.GetPropertyPath(parameter, fieldDefinition.PropertyPath);
            BinaryExpression isExact = Expression.Equal(path, searchTerm);
            MethodCallExpression isStartsWith = Expression.Call(null, ILikeMethod, efFunctions, path, startsWithTerm);
            MethodCallExpression isPartial = Expression.Call(null, ILikeMethod, efFunctions, path, partialTerm);
            MethodCallExpression isFuzzy = Expression.Call(null, TrigramsAreSimilarMethod, efFunctions, path, searchTerm);

            Expression? fieldWhere = null;
            if (fieldDefinition.AllowedSearchRules.HasFlag(MatchType.Exact))
                fieldWhere = isExact;
            if (fieldDefinition.AllowedSearchRules.HasFlag(MatchType.StartsWith))
                fieldWhere = fieldWhere is null ? isStartsWith : Expression.OrElse(fieldWhere, isStartsWith);
            if (fieldDefinition.AllowedSearchRules.HasFlag(MatchType.Partial))
                fieldWhere = fieldWhere is null ? isPartial : Expression.OrElse(fieldWhere, isPartial);
            if (fieldDefinition.AllowedSearchRules.HasFlag(MatchType.Fuzzy))
                fieldWhere = fieldWhere is null ? isFuzzy : Expression.OrElse(fieldWhere, isFuzzy);

            if (fieldWhere != null)
                combinedWhere = combinedWhere is null ? fieldWhere : Expression.OrElse(combinedWhere, fieldWhere);

            // scoring logic
            Expression fieldScore = Expression.Constant(0);

            fieldScore = Expression.Add(
                fieldScore,
                Expression.Condition(isExact,
                    Expression.Constant(3),
                    Expression.Constant(0)));

            fieldScore = Expression.Add(
                fieldScore,
                Expression.Condition(isPartial,
                    Expression.Constant(2),
                    Expression.Constant(0)));

            fieldScore = Expression.Add(
                fieldScore,
                Expression.Condition(isFuzzy,
                    Expression.Constant(1),
                    Expression.Constant(0)));

            currentScoreSum = Expression.Add(currentScoreSum, fieldScore);
        }

        return (combinedWhere is null ? e => true : Expression.Lambda<Func<TEntity, bool>>(combinedWhere, parameter),
                Expression.Lambda<Func<TEntity, int>>(currentScoreSum, parameter));
    }
}

public static class FacetExpressionBuilder<TEntity> where TEntity : class
{
    public static Expression<Func<TEntity, object>>? Build(string key)
    {
        if (!SearchRegistry.FieldMap.TryGetValue(key, out FieldDefinition? fieldDefinition))
            return null;

        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");
        Expression propertyPath = SearchRegistry.GetPropertyPath(parameter, fieldDefinition.PropertyPath);
        UnaryExpression castToObj = Expression.Convert(propertyPath, typeof(object));

        return Expression.Lambda<Func<TEntity, object>>(castToObj, parameter);
    }
}



public sealed class EfEstablishmentSearchProvider
{
    private readonly IDbContextFactory<EducationProviderRegistryDbContext> _dbContextFactory;
    private readonly int _pageSize = 50;

    public EfEstablishmentSearchProvider(IDbContextFactory<EducationProviderRegistryDbContext> dbContextFactory)
    {
        ArgumentNullException.ThrowIfNull(dbContextFactory);
        _dbContextFactory = dbContextFactory;
    }

    public async Task<(IReadOnlyList<Establishment> Data, List<SearchFacet> Facets)>
        ExecuteSearchAsync(SearchServiceAdapterRequest request, CancellationToken cancellationToken)
    {
        var (searchExp, scoreExp) = SearchExpressionBuilder<Establishment>.Build(request);
        var filterExp = FilterExpressionBbuilder<Establishment>.BuildFilters(request.SearchFilterRequests);

        Task<IReadOnlyList<Establishment>> dataTask = FetchDataAsync(request, searchExp, filterExp, scoreExp, cancellationToken);

        IEnumerable<Task<SearchFacet>> facetTasks = request.Facets
            .Select(key => FetchFacetsAsync(key, searchExp, filterExp, cancellationToken))
            .Cast<Task<SearchFacet>>();

        Task<SearchFacet[]> allFacetTasks = Task.WhenAll(facetTasks);
        IReadOnlyList<Establishment> data = await dataTask;

        List<SearchFacet> facets = (await allFacetTasks)
            .Where(f => f is not null)
            .ToList();

        return (data, facets);
    }

    private IQueryable<Establishment> BaseQuery(
        EducationProviderRegistryDbContext context,
        Expression<Func<Establishment, bool>> filters,
        Expression<Func<Establishment, bool>> searchPredicate) =>
        context.Establishment
        .AsNoTracking()
        .AsSplitQuery()
        .Include(establishemnt => establishemnt.Site)
        .Include(establishemnt => establishemnt.EstablishmentType)
        .Include(establishemnt => establishemnt.EstablishmentStatus)
        .Include(establishemnt => establishemnt.EstablishmentAuthority)
        .Include(establishemnt => establishemnt.EstablishmentGroupMembership)
            .ThenInclude(groupMembership => groupMembership.Group)
                .ThenInclude(group => group.GroupType)
        .Where(filters)
        .Where(searchPredicate);

    private async Task<IReadOnlyList<Establishment>> FetchDataAsync(
        SearchServiceAdapterRequest request,
        Expression<Func<Establishment, bool>> searchExp,
        Expression<Func<Establishment, bool>> filterExp,
        Expression<Func<Establishment, int>> scoreExp,
        CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var query = BaseQuery(db, filterExp, searchExp);

        if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            query = query.OrderByDescending(scoreExp);

        return await query.Skip(request.Offset).Take(_pageSize).ToListAsync(cancellationToken);
    }

    private async Task<SearchFacet?> FetchFacetsAsync(
        string facetKey,
        Expression<Func<Establishment, bool>> searchExp,
        Expression<Func<Establishment, bool>> filterExp,
        CancellationToken cancellationToken)
    {
        Expression<Func<Establishment, object>>? selector = FacetExpressionBuilder<Establishment>.Build(facetKey);
        if (selector is null)
            return null;

        await using EducationProviderRegistryDbContext db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var dbCounts = await BaseQuery(db, filterExp, searchExp)
            .GroupBy(selector)
            .Select(g => new
            {
                Value = g.Key != null ? g.Key.ToString() : "Unknown",
                Count = g.LongCount()
            })
            .ToListAsync(cancellationToken);

        var results = dbCounts
            .Select(c => new FacetResult(c.Value ?? "Unknown", c.Count))
            .OrderByDescending(f => f.Count)
            .ToList();

        return new SearchFacet(facetKey, results);
    }
}



public sealed class EfEstablishmentSearchServiceAdapater : ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>
{
    private readonly EfEstablishmentSearchProvider _searchProvider;
    private readonly IMapper<Establishment, EstablishmentSearchResult> _estabMapper;

    public EfEstablishmentSearchServiceAdapater(
        EfEstablishmentSearchProvider efEstablishmentSearchProvider,
        IMapper<Establishment, EstablishmentSearchResult> estabMapper)
    {
        ArgumentNullException.ThrowIfNull(efEstablishmentSearchProvider);
        ArgumentNullException.ThrowIfNull(estabMapper);

        _searchProvider = efEstablishmentSearchProvider;
        _estabMapper = estabMapper;
    }

    public async Task<SearchResults<EstablishmentSearchResults, SearchFacets>> SearchAsync(
        SearchServiceAdapterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // executre concurrent efcore search, filtering an faceting
        (IReadOnlyList<Establishment>? establishments, List<SearchFacet>? facets) = await _searchProvider
            .ExecuteSearchAsync(request, cancellationToken);

        if (establishments.Count == 0)
            return new();

        ReadOnlyCollection<EstablishmentSearchResult> estabsMapped = establishments
           .Select(_estabMapper.Map)
           .ToList()
           .AsReadOnly();

        return new SearchResults<EstablishmentSearchResults, SearchFacets>
        {
            Results = new EstablishmentSearchResults(estabsMapped),
            FacetResults = new SearchFacets(facets)
        };
    }
}
