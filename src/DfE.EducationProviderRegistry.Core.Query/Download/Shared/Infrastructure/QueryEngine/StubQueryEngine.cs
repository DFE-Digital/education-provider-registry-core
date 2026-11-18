using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.Extensions;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine;

[ExcludeFromCodeCoverage]
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
                _dbContext.EstablishmentDownloadAggregate
                    .AsNoTracking()
                    .Where(sa => sa.ProviderStatusId == 1)
                    .Select(EstablishmentDownloadAggregateExtensions.ToDto)
                    .Cast<object>(),

            "closed-provider" =>
                _dbContext.EstablishmentDownloadAggregate
                    .AsNoTracking()
                    .Where(sa => sa.ProviderStatusId == 2)
                    .Select(EstablishmentDownloadAggregateExtensions.ToDto)
                    .Cast<object>(),

            "open-provider-group" =>
                _dbContext.GroupDownloadAggregate
                    .AsNoTracking()
                    .Select(GroupDownloadAggregateExtensions.ToDto)
                    .Cast<object>(),

            _ => throw new ArgumentException($"Unknown file key: {fileKey}", nameof(fileKey))
        };
    }
}
