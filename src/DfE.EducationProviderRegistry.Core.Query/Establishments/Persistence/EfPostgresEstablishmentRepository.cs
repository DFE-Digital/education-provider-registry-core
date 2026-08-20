using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;

internal sealed class EfPostgresEstablishmentRepository : IEstablishmentsRepository
{
    private readonly EducationProviderRegistryDbContext _dbContext;
    private readonly IMapper<Establishment, EstablishmentDetailsModel> _establishmentDetailsMapper;

    public EfPostgresEstablishmentRepository(
        EducationProviderRegistryDbContext appDbContext,
        IMapper<Establishment, EstablishmentDetailsModel> establishmentDetailsMapper)
    {
        ArgumentNullException.ThrowIfNull(appDbContext);
        ArgumentNullException.ThrowIfNull(establishmentDetailsMapper);
        _dbContext = appDbContext;
        _establishmentDetailsMapper = establishmentDetailsMapper;
    }

    public async Task<EstablishmentDetailsModel?> GetEstablishmentById(
        EstablishmentUrnModel identifier,
        CancellationToken cancellationToken = default)
    {
        EstablishmentDetailsModel? result = await _dbContext.Establishment
            .AsNoTracking()
            .AsSplitQuery()
            .Where(e => e.Urn == identifier.Value)

            .Include(e => e.EstablishmentGroupMembership)
                .ThenInclude(gm => gm.Group)
                    .ThenInclude(g => g.GroupType)

            .Include(e => e.EstablishmentStatus)
            .Include(e => e.EstablishmentType)

            .Include(e => e.EstablishmentProvision)
                .ThenInclude(ep => ep.EducationPhase)

            .Include(e => e.EstablishmentLifecycleEvent)

            .Include(e => e.RoleAssignment)
                .ThenInclude(ra => ra.Role)
                    .ThenInclude(r => r.Person)

            .Include(e => e.RoleAssignment)
                .ThenInclude(ra => ra.Role)
                    .ThenInclude(r => r.RoleType)

            .Include(e => e.Site)
            .Include(e => e.EstablishmentAuthority)
            .Include(e => e.EstablishmentReligion)
            .Include(e => e.EstablishmentInspection)
            .Include(e => e.EstablishmentAdmissions)
            .Include(e => e.EstablishmentSen)
            .Include(e => e.Contact)

            .Select(e => _establishmentDetailsMapper.Map(e))
            .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<IReadOnlyCollection<EstablishmentDetailsModel>> GetEstablishments(CancellationToken cancellationToken = default)
    {
        List<EstablishmentDetailsModel> results = await _dbContext.Establishment
            .AsNoTracking()
            .Select(e => new EstablishmentDetailsModel
            {
                Urn = EstablishmentUrnModel.Create(e.Urn),
                Name = new EstablishmentNameModel(e.Name),
            })
            .ToListAsync(cancellationToken);

        return results;
    }
}
