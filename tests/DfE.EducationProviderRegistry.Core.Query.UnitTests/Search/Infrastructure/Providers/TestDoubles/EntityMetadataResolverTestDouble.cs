using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using Microsoft.EntityFrameworkCore;
using Moq;
using static DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.TrigramSearchOrchestratorUnitTests;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;


[ExcludeFromCodeCoverage]

internal static class EntityMetadataResolverTestDouble
{
    public static Mock<IEntityMetadataResolver<TestEntity>> Mock() => new(MockBehavior.Strict);

    public static Mock<IEntityMetadataResolver<TestEntity>> MockFor(EntityMetadata metadata)
    {
        Mock<IEntityMetadataResolver<TestEntity>> resolverMock = Mock();
        resolverMock
            .Setup(resolver =>
                resolver.Resolve(It.IsAny<DbContext>()))
            .Returns(metadata)
            .Verifiable();

        return resolverMock;
    }
}
