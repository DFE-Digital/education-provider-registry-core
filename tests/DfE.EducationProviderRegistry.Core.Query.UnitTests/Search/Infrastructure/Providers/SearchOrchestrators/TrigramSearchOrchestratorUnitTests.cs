using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators;

public sealed class TrigramSearchOrchestratorUnitTests
{
    public sealed class TestEntity
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsMatchingEntities_WhenSearchMatches()
    {
        // arrange
        DbContext? db = DbContextTestDouble.BuildFakeDbContext();
        EntityMetadata? metadata = EntityMetadataBuilder.BuildMetadata(db);

        Mock<ISqlExecutor<TestEntity>> sqlExecutorMock =
            SqlExecutorTestDouble.MockFor(
            [
                new TestEntity { Id = 1, Name = "alpha" }
            ]);

        TrigramSearchOrchestrator<TestEntity> orchestrator =
            new(
                EntityMetadataResolverTestDouble.MockFor(metadata).Object,
                sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = new[]
        {
            new TestEntity { Id = 1, Name = "alpha" },
            new TestEntity { Id = 2, Name = "beta" }
        }
        .AsQueryable();

        SearchOrchestratorContext context = new()
        {
            SearchTerm = "alpha",
            SearchColumn = "name",
            PageSize = 10,
            Offset = 0
        };

        // act
        IReadOnlyList<TestEntity> result =
            await orchestrator.ExecuteAsync(
                db,
                baseQuery,
                context,
                "",
                CancellationToken.None);

        // assert
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsEmptyList_WhenNoMatches()
    {
        // arrange
        DbContext? db = DbContextTestDouble.BuildFakeDbContext();
        EntityMetadata? metadata = EntityMetadataBuilder.BuildMetadata(db);

        Mock<ISqlExecutor<TestEntity>> sqlExecutorMock =
            SqlExecutorTestDouble.MockFor([]);

        TrigramSearchOrchestrator<TestEntity> orchestrator =
            new(
                EntityMetadataResolverTestDouble.MockFor(metadata).Object,
                sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = new[]
        {
            new TestEntity { Id = 1, Name = "alpha" },
            new TestEntity { Id = 2, Name = "beta" }
        }
        .AsQueryable();

        SearchOrchestratorContext context = new()
        {
            SearchTerm = "zzz",
            SearchColumn = "name",
            PageSize = 10,
            Offset = 0
        };

        // act
        IReadOnlyList<TestEntity> result =
            await orchestrator.ExecuteAsync(
                db,
                baseQuery,
                context,
                "",
                CancellationToken.None);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenColumnDoesNotExist()
    {
        // arrange
        DbContext? db = DbContextTestDouble.BuildFakeDbContext();
        EntityMetadata? metadata = EntityMetadataBuilder.BuildMetadata(db);

        Mock<ISqlExecutor<TestEntity>> sqlExecutorMock =
            SqlExecutorTestDouble.MockFor([]);

        TrigramSearchOrchestrator<TestEntity> orchestrator =
            new(
                EntityMetadataResolverTestDouble.MockFor(metadata).Object,
                sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = Array.Empty<TestEntity>().AsQueryable();

        SearchOrchestratorContext context = new()
        {
            SearchTerm = "alpha",
            SearchColumn = "nonexistent_column",
            PageSize = 10,
            Offset = 0
        };

        // act/assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.ExecuteAsync(
                db,
                baseQuery,
                context,
                "",
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenDbContextIsNull()
    {
        // arrange
        EntityMetadata? metadata =
            EntityMetadataBuilder.BuildMetadata(DbContextTestDouble.BuildFakeDbContext());

        Mock<ISqlExecutor<TestEntity>> sqlExecutorMock = SqlExecutorTestDouble.MockFor([]);

        TrigramSearchOrchestrator<TestEntity> orchestrator =
            new(
                EntityMetadataResolverTestDouble.MockFor(metadata).Object,
                sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = Array.Empty<TestEntity>().AsQueryable();

        SearchOrchestratorContext context = new()
        {
            SearchTerm = "alpha",
            SearchColumn = "name",
            PageSize = 10,
            Offset = 0
        };

        // act/assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            orchestrator.ExecuteAsync(
                null!,
                baseQuery,
                context,
                "",
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenContextIsNull()
    {
        // arrange
        DbContext? db = DbContextTestDouble.BuildFakeDbContext();
        EntityMetadata? metadata = EntityMetadataBuilder.BuildMetadata(db);

        Mock<ISqlExecutor<TestEntity>> sqlExecutorMock = SqlExecutorTestDouble.MockFor([]);

        TrigramSearchOrchestrator<TestEntity> orchestrator =
            new(
                EntityMetadataResolverTestDouble.MockFor(metadata).Object,
                sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = Array.Empty<TestEntity>().AsQueryable();

        // act/assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            orchestrator.ExecuteAsync(
                db,
                baseQuery,
                null!,
                "",
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenEntityInstanceIsNull()
    {
        // arrange
        DbContext? db = DbContextTestDouble.BuildFakeDbContext();
        EntityMetadata? metadata = EntityMetadataBuilder.BuildMetadata(db);

        Mock<ISqlExecutor<TestEntity>> sqlExecutorMock =
            SqlExecutorTestDouble.MockFor(
            [
                new TestEntity { Id = 1, Name = "alpha" }
            ]);

        TrigramSearchOrchestrator<TestEntity> orchestrator =
            new(
                EntityMetadataResolverTestDouble.MockFor(metadata).Object,
                sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = new TestEntity?[]
        {
            null,
            new() { Id = 1, Name = "alpha" }
        }
        .AsQueryable()!;

        SearchOrchestratorContext context = new()
        {
            SearchTerm = "alpha",
            SearchColumn = "name",
            PageSize = 10,
            Offset = 0
        };

        // act/assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.ExecuteAsync(
                db,
                baseQuery!,
                context,
                "",
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenPrimaryKeyPropertyDoesNotExist()
    {
        // arrange
        DbContext? db = DbContextTestDouble.BuildFakeDbContext();
        EntityMetadata? metadata = EntityMetadataBuilder.BuildMetadata(db);

        Mock<IProperty> fakePkProperty = new();
        fakePkProperty.Setup(property =>
            property.Name).Returns("DoesNotExist");

        metadata = new EntityMetadata(
            metadata.EntityType,
            metadata.Schema,
            metadata.TableName,
            fakePkProperty.Object,
            "DoesNotExist");

        Mock<ISqlExecutor<TestEntity>> sqlExecutorMock =
            SqlExecutorTestDouble.MockFor(
            [
                new TestEntity { Id = 1, Name = "alpha" }
            ]);

        TrigramSearchOrchestrator<TestEntity> orchestrator =
            new(
                EntityMetadataResolverTestDouble.MockFor(metadata).Object,
                sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = new[]
        {
            new TestEntity { Id = 1, Name = "alpha" }
        }
        .AsQueryable();

        SearchOrchestratorContext context =
            new()
            {
                SearchTerm = "alpha",
                SearchColumn = "name",
                PageSize = 10,
                Offset = 0
            };

        // act/assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.ExecuteAsync(
                db,
                baseQuery,
                context,
                "",
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_Throws_WhenPrimaryKeyValueIsNull()
    {
        // arrange
        DbContext? db = DbContextTestDouble.BuildFakeDbContext();
        EntityMetadata? metadata = EntityMetadataBuilder.BuildMetadata(db);

        Mock<ISqlExecutor<TestEntity>> sqlExecutorMock =
            SqlExecutorTestDouble.MockFor(
            [
                new TestEntity { Id = null, Name = "alpha" }
            ]);

        TrigramSearchOrchestrator<TestEntity> orchestrator =
            new(
                EntityMetadataResolverTestDouble.MockFor(metadata).Object,
                sqlExecutorMock.Object);

        IQueryable<TestEntity> baseQuery = new[]
        {
            new TestEntity { Id = null, Name = "alpha" }
        }
        .AsQueryable();

        SearchOrchestratorContext context = new()
        {
            SearchTerm = "alpha",
            SearchColumn = "name",
            PageSize = 10,
            Offset = 0
        };

        // act/assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orchestrator.ExecuteAsync(
                db,
                baseQuery,
                context,
                "",
                CancellationToken.None));
    }
}
