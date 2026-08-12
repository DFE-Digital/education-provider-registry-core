using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

/// <summary>
/// Defines a component capable of executing a search operation for
/// <typeparamref name="TEntity"/> using trigram similarity, paging, and
/// additional filter criteria.
/// </summary>
/// <typeparam name="TEntity">
/// The EF‑mapped entity or projection type being queried.
/// </typeparam>
public interface ISearchProvider<TEntity>
{
    /// <summary>
    /// Executes a search using the supplied term, paging parameters, and
    /// filter requests, returning the matching entities in ranked order.
    /// </summary>
    /// <param name="searchTerm">The term used for trigram similarity matching.</param>
    /// <param name="pageSize">The maximum number of results to return.</param>
    /// <param name="offset">The number of results to skip.</param>
    /// <param name="filters">Additional filters to apply to the search.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// A read‑only list of matching <typeparamref name="TEntity"/> instances.
    /// </returns>
    Task<IReadOnlyList<TEntity>> GetMatchingIdsAsync(
        string searchTerm,
        int pageSize,
        int offset,
        IReadOnlyList<SearchFilterRequest> filters,
        CancellationToken cancellationToken = default);
}
