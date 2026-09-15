using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

internal sealed class SearchAggregateSeeder : ISearchAggregateSeeder
{
    private readonly EducationProviderRegistryDbContext _dbContext;

    public SearchAggregateSeeder(EducationProviderRegistryDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(ct);

        await _dbContext.SearchAggregate.ExecuteDeleteAsync(ct);
        await transaction.CommitAsync(ct);
    }

    public async Task<SearchableAggregates> SeedAsync(IReadOnlyCollection<SearchAggregate> searchAggregates, CancellationToken ct = default)
    {
        await InsertSearchAggregatesAsync(_dbContext, [.. searchAggregates], ct);

        // Requery for updated values as mapping assertions require
        List<string?> matchIds = searchAggregates.Select(x => x.ProviderId).ToList();

        IReadOnlyCollection<SearchAggregate> rehydratedMatches =
            await _dbContext.SearchAggregate
                .Where(x => matchIds.Contains(x.ProviderId))
                .ToListAsync(ct);

        return new()
        {
            SearchAggregates = rehydratedMatches,
        };
    }

    private static async Task InsertSearchAggregatesAsync(
        EducationProviderRegistryDbContext dbContext,
        IReadOnlyCollection<SearchAggregate> searchAggregates,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(searchAggregates);

        if (searchAggregates.Count == 0)
            return;

        await dbContext.BulkInsertAsync(
            searchAggregates,
            bulkConfig: new BulkConfig()
            {
                SetOutputIdentity = true,
                // IncludeGraph = true (For large datasets this takes a lot of time)
            },
            cancellationToken: ct);
    }
}
