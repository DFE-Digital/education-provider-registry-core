using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;

public sealed class FacetAggregator : IFacetAggregator
{
    private readonly IFacetProvider _provider;

    public FacetAggregator(IFacetProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        _provider = provider;
    }

    public async Task<IReadOnlyList<AggregatedFacetResult>> CalculateFacetsAsync(
        IQueryable<SearchAggregate> matchingQuery,
        IEnumerable<string>? requestedFacets,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(matchingQuery);

        if (requestedFacets is null || !requestedFacets.Any())
        {
            return [];
        }

        List<AggregatedFacetResult> results = [];

        foreach (string facet in requestedFacets.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            IReadOnlyList<FacetResult> providerResults =
                await _provider.GetFacetsAsync(
                    matchingQuery,
                    facet,
                    cancellationToken);

            results.Add(new AggregatedFacetResult(
                facet,
                providerResults));
        }

        return results;
    }
}
