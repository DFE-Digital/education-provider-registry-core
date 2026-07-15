using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using static DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments.EstablishmentBuilder;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

internal sealed class SearchEstablishmentFactory : ISearchEstablishmentFactory
{
    private readonly EducationProviderRegistryDbContext _dbContext;

    public SearchEstablishmentFactory(EducationProviderRegistryDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);
        _dbContext = dbContext;
    }

    public async Task<SearchableEstablishments> CreateManyAsync(int totalToCreate, string searchTerm, SearchByNameMatchTerms matches, CancellationToken ct = default)
    {
        IReadOnlyCollection<Establishment> matching = CreateMatchingEstablishments(matches);

        IReadOnlyCollection<Establishment> nonMatching = CreateNotMatchingEstablishments(createCount: (totalToCreate - matches.matchingNames.Count));

        await InsertEstablishmentsAsync(
            _dbContext,
            [
                .. matching,
                .. nonMatching
            ],
            ct);


        // Requery for updated values as mapping assertions require
        List<long> matchIds = [.. matching.Select(x => x.EstablishmentId)];

        IReadOnlyCollection<Establishment> rehydratedMatches =
            await _dbContext.Establishment
                .Include(x => x.EstablishmentType)
                .Include(x => x.EstablishmentAuthority)
                .Include(x => x.Site)
                .Where(x => matchIds.Contains(x.EstablishmentId))
                .ToListAsync(ct);

        return new()
        {
            SearchTermMatches = rehydratedMatches,
        };
    }

    private List<Establishment> CreateMatchingEstablishments(SearchByNameMatchTerms config)
    {
        List<Establishment> matchedEstablishments = [];
        foreach (string matchName in config.matchingNames)
        {
            matchedEstablishments.Add(
                SearchEstablishmentBuilder
                    .Create()
                    .WithName(matchName)
                    .Build());
        }
        return matchedEstablishments;
    }

    private static List<Establishment> CreateNotMatchingEstablishments(int createCount)
    {
        List<Establishment> notMatchEstablishments = [];
        for (int count = 0; count < createCount; count++)
        {
            notMatchEstablishments.Add(
                SearchEstablishmentBuilder.Create()
                    .WithName($"ZZZ-{count}")
                    .Build());
        }

        return notMatchEstablishments;
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
            establishment.EstablishmentTypeId =
                establishmentTypeId;

            establishment.EstablishmentStatusId =
                establishmentStatusId;
        }

        // Insert everything in one batch
        await dbContext.BulkInsertAsync(
            establishments,
            bulkConfig: new BulkConfig()
            {
                SetOutputIdentity = true
            },
            cancellationToken: ct);
    }
}
