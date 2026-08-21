using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Factories;

public sealed class SearchFilterSpecificationFactory<TProjection>
    : ISearchFilterSpecificationFactory<TProjection>
    where TProjection : class
{
    private readonly Dictionary<
        string,
        Func<ISearchFilter<TProjection>>> _filterRegistry;

    public SearchFilterSpecificationFactory(
        Dictionary<
            string,
            Func<ISearchFilter<TProjection>>> filterRegistry)
    {
        _filterRegistry =
            filterRegistry
            ?? throw new ArgumentNullException(nameof(filterRegistry));
    }

    public ISpecification<TProjection> Create(
        string filterName,
        SearchFilterRequest request)
    {
        if (!_filterRegistry.TryGetValue(
            filterName,
            out Func<ISearchFilter<TProjection>>? factory))
        {
            throw new ArgumentOutOfRangeException(
                nameof(filterName),
                $"Filter '{filterName}' is not registered.");
        }

        ISearchFilter<TProjection> filter = factory();

        return filter.CreateSpecification(request);
    }
}
