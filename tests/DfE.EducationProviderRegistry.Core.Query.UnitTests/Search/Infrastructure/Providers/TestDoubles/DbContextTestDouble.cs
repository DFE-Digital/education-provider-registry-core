using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class DbContextTestDouble
{
    public static DbContext BuildFakeDbContext() =>
        new Mock<DbContext>(
            new DbContextOptions<DbContext>()).Object;
}
