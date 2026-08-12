using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Context;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators;

public sealed class TrigramSearchOrchestratorUnitTests
{
    private static TrigramSearchOrchestrator<TestEntity> BuildOrchestrator(
        IReadOnlyList<TestEntity> sqlResults,
        string translatorResponse,
        EntityMetadata? metadataOverride = null)
    {
        DbContext dbContext = DbContextTestDouble.BuildFakeDbContext();
        EntityMetadata metadata = metadataOverride ?? EntityMetadataBuilder.BuildMetadata(dbContext);

        Mock<ISqlExecutor<TestEntity>> sqlExecutorMock =
            SqlExecutorTestDouble.MockFor(sqlResults);

        Mock<ISqlFilterExpressionTranslator<TestEntity>> translatorMock =
            SqlFilterExpressionTranslatorTestDouble.MockFor<TestEntity>(
                metadata,
                response: translatorResponse);

        return new TrigramSearchOrchestrator<TestEntity>(
            EntityMetadataResolverTestDouble.MockFor(metadata).Object,
            translatorMock.Object,
            sqlExecutorMock.Object);
    }

    private static IQueryable<TestEntity> Query(
        params TestEntity?[] items) =>
            items.AsQueryable()!;

    private static SearchOrchestratorContext<TestEntity> Ctx(
        string term,
        string column,
        Expression<Func<TestEntity, bool>> filter)
    {
        return new SearchOrchestratorContext<TestEntity>
        {
            SearchTerm = term,
            SearchColumn = column,
            FilterExpression = filter,
            PageSize = 10,
            Offset = 0
        };
    }

    private static EntityMetadata OverridePk(EntityMetadata metadata, string pkName)
    {
        Mock<IProperty> fakePkProperty = new();
        fakePkProperty.Setup(property => property.Name).Returns(pkName);

        return new EntityMetadata(
            metadata.EntityType,
            metadata.Schema,
            metadata.TableName,
            fakePkProperty.Object,
            pkName);
    }

    public sealed class TestEntity
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsMatchingEntities_WhenSearchMatches()
    {
        // arrange
        TrigramSearchOrchestrator<TestEntity> orchestrator =
            BuildOrchestrator(
                new[] { new TestEntity { Id = 1, Name = "alpha" } },
                "TRUE");

        DbContext dbContext = DbContextTestDouble.BuildFakeDbContext();

        IQueryable<TestEntity> baseQuery = Query(
            new TestEntity { Id = 1, Name = "alpha" },
            new TestEntity { Id = 2, Name = "beta" });

        SearchOrchestratorContext<TestEntity> context =
            Ctx("alpha", "name", entity => true);

        // act
        IReadOnlyList<TestEntity> result =
            await orchestrator.ExecuteAsync(
                dbContext,
                baseQuery,
                context,
                CancellationToken.None);

        // assert
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsEmptyList_WhenNoMatches()
    {
        // arrange
        TrigramSearchOrchestrator<TestEntity> orchestrator =
            BuildOrchestrator(
                Array.Empty<TestEntity>(),
                "TRUE");

        DbContext dbContext = DbContextTestDouble.BuildFakeDbContext();

        IQueryable<TestEntity> baseQuery = Query(
            new TestEntity { Id = 1, Name = "alpha" },
            new TestEntity { Id = 2, Name = "beta" });

        SearchOrchestratorContext<TestEntity> context =
            Ctx("zzz", "name", entity => true);

        // act
        IReadOnlyList<TestEntity> result =
            await orchestrator.ExecuteAsync(
                dbContext,
                baseQuery,
                context,
                CancellationToken.None);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenColumnDoesNotExist()
    {
        // arrange
        TrigramSearchOrchestrator<TestEntity> orchestrator =
            BuildOrchestrator(
                Array.Empty<TestEntity>(),
                "TRUE");

        DbContext dbContext = DbContextTestDouble.BuildFakeDbContext();
        IQueryable<TestEntity> baseQuery = Query();

        SearchOrchestratorContext<TestEntity> context =
            Ctx("alpha", "nonexistent_column", entity => true);

        // act + assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.ExecuteAsync(
                dbContext,
                baseQuery,
                context,
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenDbContextIsNull()
    {
        // arrange
        TrigramSearchOrchestrator<TestEntity> orchestrator =
            BuildOrchestrator(
                Array.Empty<TestEntity>(),
                "TRUE");

        IQueryable<TestEntity> baseQuery = Query();
        SearchOrchestratorContext<TestEntity> context =
            Ctx("alpha", "name", entity => true);

        // act + assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            orchestrator.ExecuteAsync(
                null!,
                baseQuery,
                context,
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenContextIsNull()
    {
        // arrange
        TrigramSearchOrchestrator<TestEntity> orchestrator =
            BuildOrchestrator(
                Array.Empty<TestEntity>(),
                "TRUE");

        DbContext dbContext = DbContextTestDouble.BuildFakeDbContext();
        IQueryable<TestEntity> baseQuery = Query();

        // act + assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            orchestrator.ExecuteAsync(
                dbContext,
                baseQuery,
                null!,
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenEntityInstanceIsNull()
    {
        // arrange
        TrigramSearchOrchestrator<TestEntity> orchestrator =
            BuildOrchestrator(
                new[] { new TestEntity { Id = 1, Name = "alpha" } },
                "TRUE");

        DbContext dbContext = DbContextTestDouble.BuildFakeDbContext();

        IQueryable<TestEntity> baseQuery = Query(
            null,
            new TestEntity { Id = 1, Name = "alpha" });

        SearchOrchestratorContext<TestEntity> context =
            Ctx("alpha", "name", entity => true);

        // act + assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.ExecuteAsync(
                dbContext,
                baseQuery,
                context,
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenPrimaryKeyPropertyDoesNotExist()
    {
        // arrange
        DbContext dbContext = DbContextTestDouble.BuildFakeDbContext();
        EntityMetadata metadata = EntityMetadataBuilder.BuildMetadata(dbContext);

        EntityMetadata overriddenMetadata = OverridePk(metadata, "DoesNotExist");

        TrigramSearchOrchestrator<TestEntity> orchestrator =
            BuildOrchestrator(
                new[] { new TestEntity { Id = 1, Name = "alpha" } },
                "TRUE",
                overriddenMetadata);

        IQueryable<TestEntity> baseQuery = Query(
            new TestEntity { Id = 1, Name = "alpha" });

        SearchOrchestratorContext<TestEntity> context =
            Ctx("alpha", "name", entity => true);

        // act + assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.ExecuteAsync(
                dbContext,
                baseQuery,
                context,
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenPrimaryKeyValueIsNull()
    {
        // arrange
        TrigramSearchOrchestrator<TestEntity> orchestrator =
            BuildOrchestrator(
                new[] { new TestEntity { Id = null, Name = "alpha" } },
                "TRUE");

        DbContext dbContext = DbContextTestDouble.BuildFakeDbContext();

        IQueryable<TestEntity> baseQuery = Query(
            new TestEntity { Id = null, Name = "alpha" });

        SearchOrchestratorContext<TestEntity> context =
            Ctx("alpha", "name", entity => true);

        // act + assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.ExecuteAsync(
                dbContext,
                baseQuery,
                context,
                CancellationToken.None));
    }
}
