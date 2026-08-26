using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

internal sealed class SearchEstablishmentSeeder : ISearchEstablishmentSeeder
{
    private readonly EducationProviderRegistryDbContext _dbContext;

    public SearchEstablishmentSeeder(EducationProviderRegistryDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }
    public async Task ClearAsync(CancellationToken ct = default)
    {
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(ct);

        await _dbContext.EstablishmentAuthority.ExecuteDeleteAsync(ct);
        await _dbContext.Site.ExecuteDeleteAsync(ct);
        await _dbContext.Establishment.ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }

    public async Task<SearchableEstablishments> SeedAsync(IReadOnlyCollection<Establishment> establishments, CancellationToken ct = default)
    {
        await InsertEstablishmentsAsync(_dbContext, [.. establishments], ct);

        // Requery for updated values as mapping assertions require
        List<long> matchIds = [.. establishments.Select(x => x.EstablishmentId)];

        IReadOnlyCollection<Establishment> rehydratedMatches =
            await _dbContext.Establishment
                .Include(x => x.EstablishmentType)
                .Include(x => x.EstablishmentAuthority)
                .Include(x => x.Site)
                .Where(x => matchIds.Contains(x.EstablishmentId))
                .ToListAsync(ct);

        return new()
        {
            Establishments = rehydratedMatches,
        };
    }

    private static async Task InsertEstablishmentsAsync(
        EducationProviderRegistryDbContext dbContext,
        IReadOnlyCollection<Establishment> establishments,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(establishments);

        if (establishments.Count == 0)
        {
            return;
        }

        // Resolve reference data once
        EstablishmentReferenceData referenceData = new();

        long establishmentTypeId =
            await dbContext.GetEstablishmentTypeIdAsync(referenceData.EstablishmentTypeCode);

        long establishmentStatusId =
            await dbContext.GetEstablishmentStatusIdAsync(referenceData.EstablishmentStatusCode);

        // Configure all graphs
        foreach (Establishment establishment in establishments)
        {
            if (establishment.EstablishmentTypeId == default) // if not set by client
            {
                establishment.EstablishmentTypeId = establishmentTypeId;
            }

            establishment.EstablishmentStatusId =
                establishmentStatusId;
        }

        // Batch insert Establishments Note: this does not automatically include entity-relationships
        await dbContext.BulkInsertAsync(
            establishments,
            bulkConfig: new BulkConfig()
            {
                SetOutputIdentity = true,
                // IncludeGraph = true (For large datasets this takes a lot of time)
            },
            cancellationToken: ct);

        // Set relationship from EstablishmentAuthority -> Establishment for BulkInsert
        foreach (Establishment establishment in
            establishments.Where(t => t.EstablishmentAuthority.Count > 0))
        {
            foreach (EstablishmentAuthority authority in establishment.EstablishmentAuthority)
            {
                authority.EstablishmentId = establishment.EstablishmentId;
            }
        }

        // Batch insert all EstablishmentAuthority (LocalAuthorities)
        List<EstablishmentAuthority> authorities =
        [
            ..establishments
                .Where(t => t.EstablishmentAuthority.Count > 0)
                .SelectMany(e => e.EstablishmentAuthority)
        ];

        await dbContext.BulkInsertAsync(authorities, cancellationToken: ct);
    }
}
