using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Context;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers;

public sealed class EstablishmentsSearchProviderUnitTests
{
    private static Expression<Func<Establishment, bool>> TrueExpr =>
        value => true;

    private static EducationProviderRegistryDbContext Db() =>
        EducationProviderRegistryDbContextFactory.CreateDbContext();

    private static Mock<IDbContextFactory<EducationProviderRegistryDbContext>> DbFactory(
        EducationProviderRegistryDbContext db) =>
            IDbContextFactoryTestDouble.MockFor(db);

    private static IQueryable<Establishment> Query(
        params Establishment?[] items) =>
            items.AsQueryable()!;

    private static Mock<ISearchProjectionBuilder<Establishment>> Projection(
        EducationProviderRegistryDbContext db,
        IQueryable<Establishment> baseQuery) =>
            SearchProjectionBuilderTestDouble.MockFor(db, baseQuery);

    private static Mock<ISearchOrchestrator<Establishment>> Orchestrator() =>
        SearchOrchestratorTestDouble.Mock();

    private static Mock<ISearchFilterExpressionsBuilder<Establishment>> FilterBuilder(
        Expression<Func<Establishment, bool>> expr) =>
            SearchFilterExpressionsBuilderTestDouble.MockFor(expr);

    private static EstablishmentsSearchProvider Provider(
        Mock<IDbContextFactory<EducationProviderRegistryDbContext>> factory,
        Mock<ISearchOrchestrator<Establishment>> orchestrator,
        Mock<ISearchProjectionBuilder<Establishment>> projection,
        Mock<ISearchFilterExpressionsBuilder<Establishment>> filterBuilder,
        string searchColumn) =>
            new(
                factory.Object,
                orchestrator.Object,
                projection.Object,
                filterBuilder.Object,
                searchColumn);

    [Fact]
    public async Task GetMatchingIdsAsync_DelegatesToOrchestrator_WithCorrectParameters()
    {
        // arrange
        EducationProviderRegistryDbContext dbContext = Db();
        Mock<IDbContextFactory<EducationProviderRegistryDbContext>> factory = DbFactory(dbContext);

        IQueryable<Establishment> baseQuery = Query();

        Mock<ISearchProjectionBuilder<Establishment>> projectionBuilder =
            Projection(dbContext, baseQuery);

        Mock<ISearchOrchestrator<Establishment>> orchestrator = Orchestrator();

        Expression<Func<Establishment, bool>> filterExpression =
            entity => entity.EstablishmentTypeId == 1;

        Mock<ISearchFilterExpressionsBuilder<Establishment>> filterBuilder =
            FilterBuilder(filterExpression);

        EstablishmentsSearchProvider provider =
            Provider(factory, orchestrator, projectionBuilder, filterBuilder, "name");

        List<SearchFilterRequest> filters =
            [
                new("Type", new List<string> { "Academy" })
            ];

        List<Establishment> expectedResults =
            [
                new() { EstablishmentId = 1, Name = "A School" }
            ];

        orchestrator
            .Setup(searchOrchestrator =>
                searchOrchestrator.ExecuteAsync(
                    dbContext,
                    baseQuery,
                    It.IsAny<SearchOrchestratorContext<Establishment>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResults);

        // act
        IReadOnlyList<Establishment> results =
            await provider.GetMatchingIdsAsync(
                "academy",
                20,
                40,
                filters,
                CancellationToken.None);

        // assert
        Assert.Single(results);
        Assert.Equal("A School", results[0].Name);

        orchestrator.Verify(o => o.ExecuteAsync(
            dbContext,
            baseQuery,
            It.Is<SearchOrchestratorContext<Establishment>>(ctx =>
                ctx.SearchColumn == "name" &&
                ctx.SearchTerm == "academy" &&
                ctx.PageSize == 20 &&
                ctx.Offset == 40 &&
                ctx.FilterExpression == filterExpression),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMatchingIdsAsync_UsesTrueExpression_WhenNoFiltersProvided()
    {
        // arrange
        EducationProviderRegistryDbContext dbContext = Db();
        Mock<IDbContextFactory<EducationProviderRegistryDbContext>> factory = DbFactory(dbContext);

        IQueryable<Establishment> baseQuery = Query();

        Mock<ISearchProjectionBuilder<Establishment>> projectionBuilder =
            Projection(dbContext, baseQuery);

        Mock<ISearchOrchestrator<Establishment>> orchestrator = Orchestrator();

        Mock<ISearchFilterExpressionsBuilder<Establishment>> filterBuilder =
            FilterBuilder(TrueExpr);

        EstablishmentsSearchProvider provider =
            Provider(factory, orchestrator, projectionBuilder, filterBuilder, "urn");

        orchestrator
            .Setup(searchOrchestrator =>
                searchOrchestrator.ExecuteAsync(
                    dbContext,
                    baseQuery,
                    It.IsAny<SearchOrchestratorContext<Establishment>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Establishment>());

        // act
        IReadOnlyList<Establishment> results =
            await provider.GetMatchingIdsAsync(
                "10001",
                10,
                0,
                new List<SearchFilterRequest>(),
                CancellationToken.None);

        // assert
        Assert.Empty(results);

        orchestrator.Verify(searchOrchestrator =>
            searchOrchestrator.ExecuteAsync(
                dbContext,
                baseQuery,
                It.Is<SearchOrchestratorContext<Establishment>>(ctx =>
                    ctx.SearchColumn == "urn" &&
                    ctx.SearchTerm == "10001" &&
                    ctx.FilterExpression.Body is ConstantExpression &&
                    ((ConstantExpression)ctx.FilterExpression.Body).Value != null &&
                    ((ConstantExpression)ctx.FilterExpression.Body).Value!.Equals(true)),
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task GetMatchingIdsAsync_CreatesDbContext_FromFactory()
    {
        // arrange
        EducationProviderRegistryDbContext dbContext = Db();
        Mock<IDbContextFactory<EducationProviderRegistryDbContext>> factory = DbFactory(dbContext);

        IQueryable<Establishment> baseQuery = Query();

        Mock<ISearchProjectionBuilder<Establishment>> projectionBuilder =
            Projection(dbContext, baseQuery);

        Mock<ISearchOrchestrator<Establishment>> orchestrator = Orchestrator();

        orchestrator
            .Setup(searchOrchestrator =>
                searchOrchestrator.ExecuteAsync(
                    dbContext,
                    It.IsAny<IQueryable<Establishment>>(),
                    It.IsAny<SearchOrchestratorContext<Establishment>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Establishment>());

        Mock<ISearchFilterExpressionsBuilder<Establishment>> filterBuilder =
            FilterBuilder(TrueExpr);

        EstablishmentsSearchProvider provider =
            Provider(factory, orchestrator, projectionBuilder, filterBuilder, "name");

        // act
        await provider.GetMatchingIdsAsync(
            "test",
            10,
            0,
            new List<SearchFilterRequest>(),
            CancellationToken.None);

        // assert
        factory.Verify(dbContextFactory =>
            dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
