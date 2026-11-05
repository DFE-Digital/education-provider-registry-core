using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.DataTransferObjects;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine;

public sealed class StubQueryEngine : IQueryEngine
{
    private readonly EducationProviderRegistryDbContext _dbContext;

    public StubQueryEngine(EducationProviderRegistryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<object> BuildQuery(string fileKey)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(fileKey);

        return fileKey switch
        {
            "open-provider" =>
                _dbContext.EstablishmentAggregate
                    .AsNoTracking()
                    .Where(sa => sa.StatusCode == 1)
                    .Select(sa => new EstablishmentDownloadDto
                    {
                        EstablishmentUniqueReferenceNumberUrn = sa.Urn,
                        EstablishmentNumber = sa.EstablishmentNumber,
                        EstablishmentName = sa.Name,
                        EstablishmentTypeName = sa.EstablishmentTypeName,
                        EducationPhaseName = sa.EducationPhaseName
                    })
                    .Cast<object>(),

            "closed-provider" =>
                _dbContext.EstablishmentAggregate
                    .AsNoTracking()
                    .Where(sa => sa.StatusCode == 0)
                    .Select(sa => new EstablishmentDownloadDto
                    {
                        EstablishmentUniqueReferenceNumberUrn = sa.Urn,
                        EstablishmentNumber = sa.EstablishmentNumber,
                        EstablishmentName = sa.Name,
                        EstablishmentTypeName = sa.EstablishmentTypeName,
                        EducationPhaseName = sa.EducationPhaseName
                    })
                    .Cast<object>(),

            "open-provider-group" =>
                _dbContext.GroupAggregate
                    .AsNoTracking()
                    .Select(ga => new GroupDownloadDto
                    {
                        GroupUniqueIdentifierUID = ga.Ukprn,
                        GroupId = ga.GroupId,
                        GroupName = ga.Name,
                        GroupType = ga.GroupTypeName
                    })
                    .Cast<object>(),

            _ => throw new ArgumentException($"Unknown file key: {fileKey}", nameof(fileKey))
        };
    }
}
