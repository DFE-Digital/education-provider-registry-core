using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;

internal sealed class EfPostgresEstablishmensRepository : IEstablishmentsRepository
{
    private readonly EducationProviderRegistryDbContext _dbContext;
    private readonly IMapper<Establishment, EstablishmentDetailsModel> _establishmentMapper;

    public EfPostgresEstablishmensRepository(
        EducationProviderRegistryDbContext appDbContext,
        IMapper<Establishment, EstablishmentDetailsModel> establishmentMapper)
    {
        ArgumentNullException.ThrowIfNull(appDbContext);
        ArgumentNullException.ThrowIfNull(establishmentMapper);
        _dbContext = appDbContext;
        _establishmentMapper = establishmentMapper;
    }


    public async Task<EstablishmentDetailsModel?> GetEstablishmentById(EstablishmentUrnModel identifier, CancellationToken cancellationToken = default)
    {
        EstablishmentDetailsModel? result = await _dbContext.Establishment
            .AsNoTracking()
            .Where(e => e.Urn == identifier.Value)
            .Select(e => _establishmentMapper.Map(e))
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
