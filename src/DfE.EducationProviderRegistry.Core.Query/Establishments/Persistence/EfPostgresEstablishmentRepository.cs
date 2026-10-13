using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence;

internal sealed class EfPostgresEstablishmentRepository : IEstablishmentsRepository
{
    private readonly EducationProviderRegistryDbContext _dbContext;
    private readonly IMapper<EstablishmentReadModel, EstablishmentDetailsModel> _establishmentDetailsMapper;

    public EfPostgresEstablishmentRepository(
        EducationProviderRegistryDbContext appDbContext,
        IMapper<EstablishmentReadModel, EstablishmentDetailsModel> establishmentDetailsMapper)
    {
        ArgumentNullException.ThrowIfNull(appDbContext);
        ArgumentNullException.ThrowIfNull(establishmentDetailsMapper);

        _dbContext = appDbContext;
        _establishmentDetailsMapper = establishmentDetailsMapper;
    }

    public async Task<EstablishmentDetailsModel?> GetEstablishmentById(EstablishmentUrnModel identifier, CancellationToken cancellationToken = default)
    {
        EstablishmentAggregate? establishment = await _dbContext.EstablishmentAggregate
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.Urn == identifier.Value,
                cancellationToken);

        if (establishment is null)
        {
            return null;
        }

        List<EstablishmentGovernorAggregate> governors = await _dbContext.EstablishmentGovernorAggregate
            .AsNoTracking()
            .Where(g => g.EstablishmentUrn == identifier.Value)
            .ToListAsync(cancellationToken);

        EstablishmentReadModel readModel = new(establishment, governors);

        return _establishmentDetailsMapper.Map(readModel);
    }

    public async Task<IReadOnlyCollection<EstablishmentDetailsModel>> GetEstablishments(CancellationToken cancellationToken = default)
    {
        List<EstablishmentAggregate> establishments =
            await _dbContext.EstablishmentAggregate
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        List<string> urns =
            [.. establishments.Select(x => x.Urn)];

        List<EstablishmentGovernorAggregate> governors =
            await _dbContext.EstablishmentGovernorAggregate
                .AsNoTracking()
                .Where(x => urns.Contains(x.EstablishmentUrn))
                .ToListAsync(cancellationToken);

        ILookup<string, EstablishmentGovernorAggregate> governorsByUrn =
            governors.ToLookup(x => x.EstablishmentUrn);

        List<EstablishmentDetailsModel> results =
        [
            .. establishments.Select(establishment =>
                _establishmentDetailsMapper.Map(
                    new EstablishmentReadModel(
                        establishment,
                        governorsByUrn[establishment.Urn].ToArray())))
        ];

        return results;
    }
}
