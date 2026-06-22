using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;

internal sealed class EfPostgresEstablishmensRepository : IEstablishmentsRepository
{
    private readonly AppDbContext _dbContext;

    public EfPostgresEstablishmensRepository(AppDbContext appDbContext)
    {
        ArgumentNullException.ThrowIfNull(appDbContext);
        _dbContext = appDbContext;
    }


    public async Task<Establishment?> GetEstablishmentById(EstablishmentUrn identifier, CancellationToken cancellationToken = default)
    {
        Establishment? result = await _dbContext.Establishments
            .AsNoTracking()
            .Where(e => e.Urn == identifier.Value)
            .Select(e => new Establishment
            {
                Urn = EstablishmentUrn.Create(e.Urn),
                Name = new EstablishmentName(e.Name),
                Number = new EstablishmentNumber(e.EstablishmentNumber),
                Status = new EstablishmentStatus(e.EstablishmentStatus.Name),
                Type = new EstablishmentType(e.EstablishmentType.Name),
                Phase = new PhaseOfEducation(e.Provision.EducationPhase.Name),
                Admissions = new EstablishmentAdmissions(e.Admissions.StatutoryLowAge, e.Admissions.StatutoryHighAge),
                Addresses = e.Sites
                    .Select(s => new SiteAddress(s.Name, s.AddressLine1, s.AddressLine2, s.Town, s.County, s.Postcode)),
                Governors = new List<Governor>(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<IReadOnlyCollection<Establishment>> GetEstablishments(CancellationToken cancellationToken = default)
    {
        List<Establishment> results = await _dbContext.Establishments
            .AsNoTracking()
            .Select(e => new Establishment
            {
                Urn = EstablishmentUrn.Create(e.Urn),
                Name = new EstablishmentName(e.Name),
                Status = new EstablishmentStatus(e.EstablishmentStatus.Name),
                Type = new EstablishmentType(e.EstablishmentType.Name),
                Addresses = e.Sites
                    .Select(s => new SiteAddress(s.Name, s.AddressLine1, s.AddressLine2, s.Town, s.County, s.Postcode))
                    .ToList(),
            })
            .ToListAsync(cancellationToken);

        return results;
    }
}
