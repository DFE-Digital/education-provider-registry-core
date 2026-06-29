using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;

/// <summary>
/// Defines the contract for accessing establishment data from a persistence layer.
/// </summary>
/// <remarks>
/// Implementations may retrieve data from any source (e.g., API, database, file store),
/// but must return fully constructed and valid <see cref="EstablishmentDetailsModel"/> aggregates.
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
    /// A task that resolves to a read‑only collection of <see cref="EstablishmentDetailsModel"/> instances.
    /// </returns>
    /// <remarks>
    /// Implementations should throw an appropriate exception if the data source is unavailable
    /// or if retrieval fails unexpectedly.
    /// </remarks>
    Task<IReadOnlyCollection<EstablishmentDetailsModel>> GetEstablishments(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single establishment by its unique identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A task that resolves to the requested <see cref="EstablishmentDetailsModel"/> instance
    /// or <see langword="null"/> if no matching establishment is found.
    /// </returns>
    Task<EstablishmentDetailsModel?> GetEstablishmentById(
        EstablishmentUrnModel identifier,
        CancellationToken cancellationToken = default);
}
