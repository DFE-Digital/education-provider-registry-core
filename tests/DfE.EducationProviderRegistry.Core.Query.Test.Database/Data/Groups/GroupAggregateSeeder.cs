using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Groups;

internal sealed class GroupAggregateSeeder : ISeedDataHandler<IEnumerable<GroupAggregate>, GroupAggregates>
{
    public async Task<GroupAggregates> PersistAsync(IEnumerable<GroupAggregate> input, EducationProviderRegistryDbContext dbContext, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        List<GroupAggregate> seed = [.. input];

        if (seed.Count == 0)
        {
            return new GroupAggregates(values: []);
        }

        await dbContext.BulkInsertAsync(
            input,
            bulkConfig: new BulkConfig() { SetOutputIdentity = true },
            cancellationToken: ct);

        long[] seededIds = [.. input.Select(estab => estab.GroupAggregateId)];

        IQueryable<GroupAggregate> aggregate =
            dbContext.GroupAggregate.Where((group) =>
                seededIds.Contains(group.GroupAggregateId));

        return new GroupAggregates(values: await aggregate.ToArrayAsync(cancellationToken: ct));
    }
}
