using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;

internal sealed class EstablishmentAggregateSeeder : ISeedDataHandler<IEnumerable<EstablishmentAggregate>, EstablishmentAggregates>
{
    public async Task<EstablishmentAggregates> PersistAsync(IEnumerable<EstablishmentAggregate> input, EducationProviderRegistryDbContext dbContext, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        List<EstablishmentAggregate> seed = [.. input];

        if (seed.Count == 0)
        {
            return new EstablishmentAggregates(values: []);
        }

        await dbContext.BulkInsertAsync(
            input,
            bulkConfig: new BulkConfig() { SetOutputIdentity = true },
            cancellationToken: ct);

        long[] seededIds = [.. input.Select(estab => estab.EstablishmentAggregateId)];

        IQueryable<EstablishmentAggregate> aggregate =
            dbContext.EstablishmentAggregate.Where((establishment) =>
                seededIds.Contains(establishment.EstablishmentAggregateId));

        return new EstablishmentAggregates(values: await aggregate.ToArrayAsync(cancellationToken: ct));
    }
}
