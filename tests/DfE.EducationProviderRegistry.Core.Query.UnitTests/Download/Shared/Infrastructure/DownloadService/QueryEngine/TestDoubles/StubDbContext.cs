using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Download.Shared.Infrastructure.DownloadService.QueryEngine.TestDoubles;

public sealed class StubDbContext : IDisposable
{
    public EducationProviderRegistryDbContext DbContext { get; }

    public StubDbContext()
    {
        DbContextOptions<EducationProviderRegistryDbContext> options =
            new DbContextOptionsBuilder<EducationProviderRegistryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        DbContext = new EducationProviderRegistryDbContext(options);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        DbContext.EstablishmentDownloadAggregate.AddRange(
            new EstablishmentDownloadAggregate
            {
                Urn = 100001,
                EstablishmentNumber = "001",
                ProviderName = "Open School",
                ProviderStatusId = 1,
                ProviderType = "Academy",
                EducationPhase = "Secondary"
            },
            new EstablishmentDownloadAggregate
            {
                Urn = 100002,
                EstablishmentNumber = "002",
                ProviderName = "Closed School",
                ProviderStatusId = 2,
                ProviderType = "School",
                EducationPhase = "Primary"
            });

        DbContext.GroupDownloadAggregate.Add(
            new GroupDownloadAggregate
            {
                GroupUid = 500001,
                GroupId = "G01",
                GroupName = "Trust Alpha",
                GroupType = "Multi-Academy Trust"
            });

        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}
