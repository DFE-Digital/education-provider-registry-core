using DfE.GIAS2.Query.Service.Core.Establishments.Application.Model;

namespace DfE.GIAS2.Query.Service.Core.Establishments.Infrastructure;

/// <summary>
/// Defines the contract for accessing establishment data from a persistence layer.
/// </summary>
/// <remarks>
/// Implementations may retrieve data from any source (e.g., API, database, file store),
/// but must return fully constructed and valid <see cref="Establishment"/> aggregates.
/// </remarks>
public interface IEstablishmentsRepository
{
    /// <summary>
    /// Retrieves all establishments from the underlying data source.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A task that resolves to a read‑only collection of <see cref="Establishment"/> instances.
    /// </returns>
    /// <remarks>
    /// Implementations should throw an appropriate exception if the data source is unavailable
    /// or if retrieval fails unexpectedly.
    /// </remarks>
    Task<IReadOnlyCollection<Establishment>> GetEstablishments(
        CancellationToken cancellationToken = default);
}
