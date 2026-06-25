using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;

internal sealed class EfPostgresEstablishmensRepository : IEstablishmentsRepository
{
    private readonly EducationProviderRegistryDbContext _dbContext;

    public EfPostgresEstablishmensRepository(EducationProviderRegistryDbContext appDbContext)
    {
        ArgumentNullException.ThrowIfNull(appDbContext);
        _dbContext = appDbContext;
    }


    public async Task<Establishment?> GetEstablishmentById(EstablishmentUrn identifier, CancellationToken cancellationToken = default)
    {
        Establishment? result = await _dbContext.Establishment
            .AsNoTracking()
            .Where(e => e.Urn == identifier.Value)
            .Select(e => new Establishment
            {
                Urn = EstablishmentUrn.Create(e.Urn.ToString()),
                Name = new EstablishmentName(e.Name),
                Number = new EstablishmentNumber(e.EstablishmentNumber),
                Status = new EstablishmentStatus(e.EstablishmentStatus.Name),
                Type = new EstablishmentType(e.EstablishmentType.Name),
                Phase = new PhaseOfEducation(e.EstablishmentProvision.EducationPhase.Name),
                LifecycleEventOpened = e.EstablishmentLifecycleEvent
                    .Where(l => l.EventType == "Opened")
                    .Select(l => new EstablishmentLifecycleEvent(
                        EstablishmentLifecycleEventType.Opened,
                        l.EventDate,
                        new EstablishmentLifeCycleReason(l.OpenedReason.Name)))
                    .FirstOrDefault(), // TODO: Handle getting specific lifecycle event types in a more robust way
                LifecycleEventClosed = e.EstablishmentLifecycleEvent
                    .Where(l => l.EventType == "Closed")
                    .Select(l => new EstablishmentLifecycleEvent(
                        EstablishmentLifecycleEventType.Closed,
                        l.EventDate,
                        new EstablishmentLifeCycleReason(l.ClosedReason.Name)))
                    .FirstOrDefault(), // TODO: Handle getting specific lifecycle event types in a more robust way
                Admissions = new EstablishmentAdmissions(
                    e.EstablishmentAdmissions.StatutoryLowAge,
                    e.EstablishmentAdmissions.StatutoryHighAge),
                Addresses = e.Site
                    .Select(s => new SiteAddress(s.Name, s.AddressLine1, s.AddressLine2, s.Town, s.County, s.Postcode)),
                Governors = new List<Governor>(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<IReadOnlyCollection<Establishment>> GetEstablishments(CancellationToken cancellationToken = default)
    {
        List<Establishment> results = await _dbContext.Establishment
            .AsNoTracking()
            .Select(e => new Establishment
            {
                Urn = EstablishmentUrn.Create(e.Urn),
                Name = new EstablishmentName(e.Name),
            })
            .ToListAsync(cancellationToken);

        return results;
    }
}
