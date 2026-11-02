using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
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
        IReadOnlyCollection<FacetContext> facets,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(matchingQuery);

        if (facets is null || facets.Count == 0)
        {
            return [];
        }

        List<AggregatedFacetResult> results = [];

        foreach (FacetContext facetContext in facets)
        {
            IReadOnlyList<FacetResult> providerResults =
                await _provider.GetFacetsAsync(
                    matchingQuery,
                    facetContext.Facet,
                    [.. facetContext.Filter?.FilterValues ?? []],
                    cancellationToken);

            results.Add(new AggregatedFacetResult(
                facetContext.Facet,
                providerResults));
        }

        return results;
    }
}
