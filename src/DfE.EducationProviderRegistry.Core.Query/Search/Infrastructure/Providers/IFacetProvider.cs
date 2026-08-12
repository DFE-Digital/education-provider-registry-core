using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

/// <summary>
/// Defines a component capable of computing facet buckets for a filtered set of
/// entity identifiers, returning grouped values and their counts.
/// </summary>
public interface IFacetProvider
{
    /// <summary>
    /// Computes facet buckets for the specified facet name across the supplied
    /// list of entity identifiers.
    /// </summary>
    /// <param name="ids">The entity identifiers to include in the facet calculation.</param>
    /// <param name="facetName">The facet name whose selector should be applied.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// A read‑only list of <see cref="FacetResult"/> instances ordered by descending count.
    /// </returns>
    Task<IReadOnlyList<FacetResult>> GetFacetsAsync(
        IReadOnlyList<string> ids,
        string facetName,
        CancellationToken cancellationToken = default);
}
