using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;

/// <summary>
/// Provides access to establishment data for the GIAS2 query service.
/// This repository acts as the persistence boundary for the Establishments
/// feature, retrieving data transfer objects and mapping them into domain
/// models for use by the application layer.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EstablishmentsRepository"/> class.
/// </remarks>
/// <param name="establishmentsMapper">
/// The mapper responsible for converting collections of
/// <see cref="EstablishmentDataTransferObject"/> instances into
/// domain <see cref="Establishment"/> models.
/// </param>
public sealed class EstablishmentsRepository(
    IMapper<
        IEnumerable<EstablishmentDataTransferObject>,
        IReadOnlyCollection<Establishment>> establishmentsMapper) : IEstablishmentsRepository
{
    private readonly IMapper<
        IEnumerable<EstablishmentDataTransferObject>,
        IReadOnlyCollection<Establishment>> _establishmentsMapper = establishmentsMapper;

    /// <summary>
    /// Retrieves all establishments from the persistence layer.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that allows the caller to cancel the operation.
    /// </param>
    /// <returns>
    /// A read‑only collection of mapped <see cref="Establishment"/> domain models.
    /// </returns>
    /// <remarks>
    /// This implementation currently returns generated fake data until the
    /// SQL infrastructure is fully wired up.
    /// </remarks>
    public async Task<IReadOnlyCollection<Establishment>> GetEstablishments(
        CancellationToken cancellationToken = default)
    {
        // TEMPORARY: Fake data until SQL is wired up
        IEnumerable<EstablishmentDataTransferObject> dtos =
            FakeEstablishmentDataGenerator.Generate(100);

        return _establishmentsMapper.Map(dtos);
    }

    /// <summary>
    /// Provides temporary, in‑memory generation of establishment DTOs for
    /// development and testing purposes. This avoids the need for a live
    /// database connection while the persistence layer is under construction.
    /// </summary>
    internal static class FakeEstablishmentDataGenerator
    {
        /// <summary>
        /// Generates a collection of fake <see cref="EstablishmentDataTransferObject"/>
        /// instances with unique 6‑digit URNs.
        /// </summary>
        /// <param name="count">
        /// The number of DTOs to generate.
        /// </param>
        /// <returns>
        /// A read‑only collection of generated DTOs.
        /// </returns>
        public static IReadOnlyCollection<EstablishmentDataTransferObject> Generate(int count)
        {
            HashSet<string> urns = GenerateUniqueUrns(count);
            List<EstablishmentDataTransferObject> dtos = [];

            foreach (string urn in urns)
            {
                EstablishmentDataTransferObject dto = new()
                {
                    URN = urn
                };

                dtos.Add(dto);
            }

            return dtos.AsReadOnly();
        }

        /// <summary>
        /// Generates a set of unique 6‑digit numeric URNs.
        /// </summary>
        /// <param name="count">
        /// The number of unique URNs to generate.
        /// </param>
        /// <returns>
        /// A <see cref="HashSet{T}"/> containing unique URN strings.
        /// </returns>
        private static HashSet<string> GenerateUniqueUrns(int count)
        {
            HashSet<string> urns = [];
            Random random = new();

            while (urns.Count < count)
            {
                int number = random.Next(100000, 999999); // 6 digits
                string urn = number.ToString();
                urns.Add(urn);
            }

            return urns;
        }
    }
}
