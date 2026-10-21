using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;

public class FacetAggregator : IFacetAggregator
{
    private readonly IFacetProvider _facetProvider;

    public FacetAggregator(IFacetProvider facetProvider)
    {
        ArgumentNullException.ThrowIfNull(facetProvider);
        _facetProvider = facetProvider;
    }

    public async Task<IReadOnlyList<AggregatedFacetResult>> CalculateFacetsAsync(
        IQueryable<SearchAggregate> matchingQuery,
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
                    matchingQuery,
                    facet,
                    cancellationToken);

            results.Add(new AggregatedFacetResult(facet, providerResults));
        }

        return results;
    }
}
