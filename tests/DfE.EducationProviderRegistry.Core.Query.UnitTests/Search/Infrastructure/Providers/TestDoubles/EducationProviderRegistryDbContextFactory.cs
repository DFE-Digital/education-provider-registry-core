using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EducationProviderRegistryDbContextFactory
{
    public static EducationProviderRegistryDbContext CreateDbContext()
    {
        DbContextOptions<EducationProviderRegistryDbContext> options =
            new DbContextOptionsBuilder<EducationProviderRegistryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new EducationProviderRegistryDbContext(options);
    }
}
