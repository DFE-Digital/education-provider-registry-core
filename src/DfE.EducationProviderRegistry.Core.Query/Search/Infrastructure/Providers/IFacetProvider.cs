using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

/// <summary>
/// Computes facet buckets over an unpaginated matching query
/// entity identifiers, returning grouped values and their counts.
/// </summary>
public interface IFacetProvider
{
    /// <summary>
    /// Computes facet buckets for the specified facet name across all matching records
    /// list of entity identifiers.
    /// </summary>
    /// <param name="matchingQuery">The search query with filters applied, before pagination.</param>
    /// <param name="facetName">The facet name whose selector should be applied.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// A read‑only list of <see cref="FacetResult"/> instances ordered by descending count.
    /// </returns>
    Task<IReadOnlyList<FacetResult>> GetFacetsAsync(
        IQueryable<SearchAggregate> matchingQuery,
        string facetName,
        CancellationToken cancellationToken = default);
}
