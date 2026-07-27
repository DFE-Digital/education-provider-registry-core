using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;

public sealed class FilterSpecificationFactory<TProjection>
    : ISearchFilterSpecificationFactory<TProjection>
    where TProjection : class
{
    private readonly Dictionary<
        string,
        Func<ISearchFilterExpression<TProjection>>> _filterRegistry;

    public FilterSpecificationFactory(
        Dictionary<
            string,
            Func<ISearchFilterExpression<TProjection>>> filterRegistry)
    {
        _filterRegistry =
            filterRegistry
            ?? throw new ArgumentNullException(nameof(filterRegistry));
    }

    public ISpecification<TProjection> CreateFilter(
        string filterName,
        SearchFilterRequest request)
    {
        if (!_filterRegistry.TryGetValue(
            filterName,
            out Func<ISearchFilterExpression<TProjection>>? factory))
        {
            throw new ArgumentOutOfRangeException(
                nameof(filterName),
                $"Filter '{filterName}' is not registered.");
        }

        ISearchFilterExpression<TProjection> filter = factory();

        return filter.CreateSpecification(request);
    }
}
