using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;

/// <summary>
/// Factory responsible for resolving and composing typed filter expressions
/// for <typeparamref name="TProjection"/>. Each filter requires a
/// <see cref="SearchFilterRequest"/> to construct its predicate expression.
/// </summary>
/// <typeparam name="TProjection">The entity or projection type.</typeparam>
public sealed class FilterExpressionFactory<TProjection> : ISearchFilterExpressionFactory<TProjection>
    where TProjection : class
{
    private readonly Dictionary<string, Func<ISearchFilterExpression<TProjection>>> _filterRegistry;
    private readonly ILogicalOperatorFactory<TProjection> _logicalOperatorFactory;

    public FilterExpressionFactory(
        Dictionary<string, Func<ISearchFilterExpression<TProjection>>> filterRegistry,
        ILogicalOperatorFactory<TProjection> logicalOperatorFactory)
    {
        _filterRegistry = filterRegistry
            ?? throw new ArgumentNullException(nameof(filterRegistry));

        _logicalOperatorFactory = logicalOperatorFactory
            ?? throw new ArgumentNullException(nameof(logicalOperatorFactory));
    }

    /// <summary>
    /// Creates a single filter expression using the supplied request.
    /// </summary>
    public Expression<Func<TProjection, bool>> CreateFilter(
        string filterName,
        SearchFilterRequest request)
    {
        if (!_filterRegistry.TryGetValue(
            filterName,
            out Func<ISearchFilterExpression<TProjection>>? factory))
        {
            throw new ArgumentOutOfRangeException(
                $"Filter '{filterName}' is not registered.");
        }

        ISearchFilterExpression<TProjection> filter = factory();
        return filter.ToExpression(request);
    }

    /// <summary>
    /// Creates and composes multiple filter expressions using a logical operator.
    /// </summary>
    public Expression<Func<TProjection, bool>> ComposeFilters(
        IReadOnlyList<(
            string FilterName,
            SearchFilterRequest Request)> filters,
        string logicalOperatorName)
    {
        if (filters.Count == 0)
        {
            return projection => true;
        }

        ILogicalOperator<TProjection> logicalOperator =
            _logicalOperatorFactory.Resolve(logicalOperatorName);

        Expression<Func<TProjection, bool>> combined =
            CreateFilter(filters[0].FilterName, filters[0].Request);

        for (int i = 1; i < filters.Count; i++)
        {
            Expression<Func<TProjection, bool>> next =
                CreateFilter(filters[i].FilterName, filters[i].Request);

            combined = logicalOperator.Combine(combined, next);
        }

        return combined;
    }

    /// <summary>
    /// Composes already-instantiated filter objects using a logical operator.
    /// </summary>
    public Expression<Func<TProjection, bool>> ComposeFilters(
        IReadOnlyList<(
            ISearchFilterExpression<TProjection> Filter,
            SearchFilterRequest Request)> filters,
        string logicalOperatorName)
    {
        if (filters.Count == 0)
        {
            return projection => true;
        }

        ILogicalOperator<TProjection> logicalOperator =
            _logicalOperatorFactory.Resolve(logicalOperatorName);

        Expression<Func<TProjection, bool>> combined =
            filters[0].Filter.ToExpression(filters[0].Request);

        for (int i = 1; i < filters.Count; i++)
        {
            combined = logicalOperator.Combine(
                combined,
                filters[i].Filter.ToExpression(filters[i].Request));
        }

        return combined;
    }
}
