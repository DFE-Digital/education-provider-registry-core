using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;

public interface IFacetAggregator
{
    Task<IReadOnlyList<AggregatedFacetResult>> CalculateFacetsAsync(
        IQueryable<SearchAggregate> matchingQuery,
        IEnumerable<string>? requestedFacets,
        CancellationToken cancellationToken);
}
