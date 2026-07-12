using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class IDbContextFactoryTestDouble
{
    public static Mock<IDbContextFactory<EducationProviderRegistryDbContext>> Mock() =>
        new(MockBehavior.Strict);

    public static Mock<IDbContextFactory<EducationProviderRegistryDbContext>>
        MockFor(EducationProviderRegistryDbContext dbContext)
    {
        Mock<IDbContextFactory<EducationProviderRegistryDbContext>> contextFactoryMock = Mock();

        contextFactoryMock.
            Setup(factory =>
                factory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(dbContext)
                    .Verifiable();

        return contextFactoryMock;
    }
}
