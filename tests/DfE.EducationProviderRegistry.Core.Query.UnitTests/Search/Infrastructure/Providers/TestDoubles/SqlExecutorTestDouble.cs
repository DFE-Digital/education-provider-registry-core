using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using Microsoft.EntityFrameworkCore;
using Moq;
using static DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.TrigramSearchOrchestratorUnitTests;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SqlExecutorTestDouble
{
    public static Mock<ISqlExecutor<TestEntity>> Mock() => new(MockBehavior.Strict);

    public static Mock<ISqlExecutor<TestEntity>> MockFor(IEnumerable<TestEntity> sqlResults)
    {
        Mock<ISqlExecutor<TestEntity>> sqlExecutorMock = Mock();

        sqlExecutorMock
            .Setup(executor =>
                executor.ExecuteIdsAsync(
                    It.IsAny<DbContext>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                .. sqlResults.Select(entity => (object?)entity.Id!)
            ]);

        return sqlExecutorMock;
    }
}
