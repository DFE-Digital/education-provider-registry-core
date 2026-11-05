using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets.Providers;

public sealed class CollectionFacetResultProvider : IFacetResultProvider
{
    private readonly Func<FacetProjection, IEnumerable<string>> _selector;

    public CollectionFacetResultProvider(
        Func<FacetProjection, IEnumerable<string>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        _selector = selector;
    }

    public IReadOnlyList<FacetResult> GetFacets(
        IEnumerable<FacetProjection> projections,
        IList<object> filterValues)
    {
        ArgumentNullException.ThrowIfNull(projections);
        ArgumentNullException.ThrowIfNull(filterValues);

        IEnumerable<string> facetValues =
            projections
                .SelectMany(_selector)
                .Where(value => !string.IsNullOrEmpty(value));

        string[] selectedValues =
        [
            .. filterValues.OfType<string>()
        ];

        if (selectedValues.Length > 0)
        {
            HashSet<string> selectedValueSet =
                new(
                    selectedValues,
                    StringComparer.Ordinal);

            facetValues =
                facetValues.Where(
                    selectedValueSet.Contains);
        }

        return
        [
            .. facetValues
                .GroupBy(
                    value => value,
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
