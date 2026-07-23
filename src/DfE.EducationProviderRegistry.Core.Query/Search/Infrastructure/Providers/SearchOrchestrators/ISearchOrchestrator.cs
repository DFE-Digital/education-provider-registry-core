using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Context;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;

/// <summary>
/// Executes the search pipeline for <typeparamref name="TProjection"/> by applying
/// similarity scoring, expression‑tree filtering, ordering, and paging over an EF Core
/// queryable source.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type being queried.
/// </typeparam>
public interface ISearchOrchestrator<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Runs the search pipeline using the supplied EF Core <paramref name="db"/> context,
    /// base LINQ <paramref name="baseQuery"/>, and the configured search parameters in
    /// <paramref name="context"/>. The orchestrator applies:
    /// <list type="bullet">
    /// <item><description>similarity matching using the configured search column,</description></item>
    /// <item><description>expression‑tree filters from <see cref="SearchOrchestratorContext{TProjection}.FilterExpression"/>,</description></item>
    /// <item><description>ordering based on similarity score,</description></item>
    /// <item><description>paging via <c>Skip</c>/<c>Take</c>.</description></item>
    /// </list>
    /// </summary>
    /// <param name="db">
    /// The EF Core <see cref="DbContext"/> used for executing similarity queries and
    /// materialising results.
    /// </param>
    /// <param name="baseQuery">
    /// The base query used to rehydrate full <typeparamref name="TProjection"/> instances
    /// after similarity ranking has been computed.
    /// </param>
    /// <param name="context">
    /// The search configuration containing the search term, paging parameters, filter
    /// expression, and target search column.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A read‑only list of <typeparamref name="TProjection"/> instances matching the
    /// search criteria.
    /// </returns>
    Task<IReadOnlyList<TProjection>> ExecuteAsync(
        DbContext db,
        IQueryable<TProjection> baseQuery,
        SearchOrchestratorContext<TProjection> context,
        CancellationToken cancellationToken = default);
}
