using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets.Providers;

public sealed class ScalarFacetResultProvider : IFacetResultProvider
{
    private readonly Func<FacetProjection, string?> _selector;

    public ScalarFacetResultProvider(
        Func<FacetProjection, string?> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        _selector = selector;
    }

    public IReadOnlyList<FacetResult> GetFacets(
        IEnumerable<FacetProjection> projections,
        IList<object> filterValues)
    {
        ArgumentNullException.ThrowIfNull(projections);

        return
        [
            .. projections
                .Select(_selector)
                .Where(value => !string.IsNullOrEmpty(value))
                .GroupBy(
                    value => value!,
                    StringComparer.Ordinal)
                .Select(groupedFacet =>
                    new FacetResult(
                        value: groupedFacet.Key,
                        label: groupedFacet.Key,
                        count: groupedFacet.LongCount()))
                .OrderBy(facetResult => facetResult.Label)
        ];
    }
}
