using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;

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
