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
        DbContext.EstablishmentAggregate.AddRange(
            new EstablishmentAggregate
            {
                Urn = "100001",
                EstablishmentNumber = "001",
                Name = "Open School",
                StatusCode = 1,
                EstablishmentTypeName = "Academy",
                EducationPhaseName = "Secondary"
            },
            new EstablishmentAggregate
            {
                Urn = "100002",
                EstablishmentNumber = "002",
                Name = "Closed School",
                StatusCode = 2,
                EstablishmentTypeName = "School",
                EducationPhaseName = "Primary"
            });

        DbContext.GroupAggregate.Add(
            new GroupAggregate
            {
                Ukprn = "500001",
                GroupId = "G01",
                Name = "Trust Alpha",
                GroupTypeName = "Multi-Academy Trust"
            });

        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}
