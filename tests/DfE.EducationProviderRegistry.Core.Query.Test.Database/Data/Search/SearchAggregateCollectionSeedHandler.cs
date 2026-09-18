using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;

internal sealed class SearchAggregateCollectionSeedHandler : ISeedDataHandler<IEnumerable<SearchAggregate>, SearchableAggregates>
{
    public async Task<SearchableAggregates> PersistAsync(IEnumerable<SearchAggregate> input, EducationProviderRegistryDbContext dbContext, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        List<SearchAggregate> seed = [.. input];

        if (seed.Count == 0)
        {
            return SearchableAggregates.Empty();
        }

        await dbContext.BulkInsertAsync(
            seed,
            bulkConfig: new BulkConfig()
            {
                SetOutputIdentity = true,
                // IncludeGraph = true (For large datasets this takes a lot of time)
            },
            cancellationToken: ct);

        return await QueryForCreated(dbContext, seed, ct);
    }

    private static async Task<SearchableAggregates> QueryForCreated(EducationProviderRegistryDbContext dbContext, IEnumerable<SearchAggregate> aggregates, CancellationToken ct)
    {
        List<string?> matchProviderIds = [.. aggregates.Select(x => x.ProviderId)];

        IReadOnlyCollection<SearchAggregate> rehydratedMatches =
            await dbContext.SearchAggregate
                .Where(x => matchProviderIds.Contains(x.ProviderId))
                .ToListAsync(ct);

        return new()
        {
            SearchAggregates = rehydratedMatches,
        };
    }
}
