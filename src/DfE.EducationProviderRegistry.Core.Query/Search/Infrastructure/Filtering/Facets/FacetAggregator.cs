using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;

public class FacetAggregator : IFacetAggregator
{
    private readonly IFacetProvider _facetProvider;

    public FacetAggregator(IFacetProvider facetProvider)
    {
        _facetProvider = facetProvider;
    }

    public async Task<IReadOnlyList<AggregatedFacetResult>> CalculateFacetsAsync(
        IReadOnlyList<string> urns,
        IEnumerable<string>? requestedFacets,
        CancellationToken cancellationToken)
    {
        if (requestedFacets is null || !requestedFacets.Any())
            return Array.Empty<AggregatedFacetResult>();

        List<string> distinct =
            [.. requestedFacets.Distinct(StringComparer.OrdinalIgnoreCase)];

        List<AggregatedFacetResult> results = [];

        foreach (string facet in distinct)
        {
            IReadOnlyList<FacetResult> providerResults =
                await _facetProvider.GetFacetsAsync(
                    urns,
                    facet,
                    cancellationToken);

            results.Add(new AggregatedFacetResult(facet, providerResults));
        }

        return results;
    }
}
