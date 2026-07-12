using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;

/// <summary>
/// Provides access to establishment data for the GIAS2 query service.
/// This repository acts as the persistence boundary for the Establishments
/// feature, retrieving data transfer objects and mapping them into domain
/// models for use by the application layer.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="FakeDataEstablishmentsRepository"/> class.
/// </remarks>
/// <param name="establishmentsMapper">
/// The mapper responsible for converting collections of
/// <see cref="EstablishmentDto"/> instances into
/// domain <see cref="EstablishmentDetailsModel"/> models.
/// </param>
internal sealed class FakeDataEstablishmentsRepository : IEstablishmentsRepository
{
    private readonly IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentDetailsModel>> _establishmentsMapper;
    private readonly IMapper<Establishment, EstablishmentDetailsModel> _establishmentMapper;

    public FakeDataEstablishmentsRepository(
        IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentDetailsModel>> establishmentsMapper,
        IMapper<Establishment, EstablishmentDetailsModel> establishmentMapper)
    {
        ArgumentNullException.ThrowIfNull(establishmentsMapper);
        ArgumentNullException.ThrowIfNull(establishmentMapper);
        _establishmentsMapper = establishmentsMapper;
        _establishmentMapper = establishmentMapper;
    }

    /// <summary>
    /// Retrieves a single establishment by its identifier from the persistence layer.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A read‑only collection of mapped <see cref="EstablishmentDetailsModel"/> domain models
    /// or <see langword="null"/> if no matching establishment is found.
    /// </returns>
    public async Task<EstablishmentDetailsModel?> GetEstablishmentById(
        EstablishmentUrnModel identifier,
        CancellationToken cancellationToken = default)
    {
        Establishment? dto =
            FakeEstablishmentDataGenerator
            .Generate(1)
            .FirstOrDefault();

        if (dto is null)
            return null;

        return _establishmentMapper.Map(dto);
    }

    /// <summary>
    /// Retrieves all establishments from the persistence layer.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token that allows the caller to cancel the operation.
    /// </param>
    /// <returns>
    /// A read‑only collection of mapped <see cref="EstablishmentDetailsModel"/> domain models.
    /// </returns>
    /// <remarks>
    /// This implementation currently returns generated fake data until the
    /// SQL infrastructure is fully wired up.
    /// </remarks>
    public async Task<IReadOnlyCollection<EstablishmentDetailsModel>> GetEstablishments(
        CancellationToken cancellationToken = default)
    {
        // TEMPORARY: Fake data until SQL is wired up
        IEnumerable<Establishment> dtos =
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
        /// Generates a collection of fake <see cref="EstablishmentDto"/>
        /// instances with unique 6‑digit URNs.
        /// </summary>
        /// <param name="count">
        /// The number of DTOs to generate.
        /// </param>
        /// <returns>
        /// A read‑only collection of generated DTOs.
        /// </returns>
        public static IReadOnlyCollection<Establishment> Generate(int count)
        {
            HashSet<string> urns = GenerateUniqueUrns(count);
            List<Establishment> dtos = [];

            foreach (string urn in urns)
            {
                Establishment dto = new()
                {
                    Urn = urn,
                    Name = "Test School",
                    EstablishmentNumber = "123",
                    EstablishmentStatus = new EstablishmentStatus
                    {
                        Name = "Open"
                    },
                    EstablishmentType = new EstablishmentType
                    {
                        Name = "Academy"
                    },
                    EstablishmentProvision = new EstablishmentProvision
                    {
                        EducationPhase = new EducationPhase
                        {
                            Name = "Primary"
                        }
                    },
                    EstablishmentAdmissions = new EstablishmentAdmissions
                    {
                        StatutoryLowAge = 5,
                        StatutoryHighAge = 11
                    },
                    EstablishmentLifecycleEvent = new List<EstablishmentLifecycleEvent>
                    {
                        new() {
                            EventType = "Opened",
                            EventDate = new DateOnly(2000, 1, 1),
                            OpenedReason = new ReasonEstablishmentOpened
                            {
                                Name = "New School"
                            }
                        },
                        new() {
                            EventType = "Closed",
                            EventDate = new DateOnly(2020, 1, 1),
                            ClosedReason = new ReasonEstablishmentClosed
                            {
                                Name = "Merged"
                            }
                        }
                    },
                    Site = new List<Site>
                    {
                        new() {
                            Name = "Main Site",
                            AddressLine1 = "1 Test Street",
                            AddressLine2 = "Test Area",
                            Town = "Test Town",
                            County = "Test County",
                            Postcode = "TE1 1ST"
                        }
                    }
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
