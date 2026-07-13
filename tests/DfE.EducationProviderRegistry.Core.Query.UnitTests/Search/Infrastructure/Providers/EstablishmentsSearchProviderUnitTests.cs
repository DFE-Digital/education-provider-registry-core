using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline.Steps.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers;

public sealed class EstablishmentsSearchProviderUnitTests
{
    [Fact]
    public async Task GetMatchingIdsAsync_DelegatesToOrchestrator_WithCorrectParameters()
    {
        // arrange
        EducationProviderRegistryDbContext dbContext =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        Mock<IDbContextFactory<EducationProviderRegistryDbContext>> factory =
            IDbContextFactoryTestDouble.MockFor(dbContext);

        IQueryable<Establishment> baseQuery =
            new List<Establishment>().AsQueryable();

        Mock<ISearchProjectionBuilder<Establishment>> projectionBuilder =
            SearchProjectionBuilderTestDouble.MockFor(dbContext, baseQuery);

        Mock<ISearchOrchestrator<Establishment>> orchestrator =
            SearchOrchestratorTestDouble.Mock();

        Mock<ISearchFilterExpressionsBuilder> filterBuilder =
            SearchFilterExpressionsBuilderTestDouble.MockFor("type = 'Academy'");

        EstablishmentsSearchProvider provider =
            new(
                factory.Object,
                orchestrator.Object,
                projectionBuilder.Object,
                filterBuilder.Object,
                "name");

        List<SearchFilterRequest> filters =
            [
                new SearchFilterRequest("Type", new List<string> { "Academy" })
            ];

        List<Establishment> expectedResults =
            [
                new Establishment { EstablishmentId = 1, Name = "A School" }
            ];

        orchestrator.Setup(o => o.ExecuteAsync(
                dbContext,
                baseQuery,
                It.IsAny<SearchOrchestratorContext>(),
                " AND type = 'Academy'",
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

        orchestrator.Verify(orchestrator =>
            orchestrator.ExecuteAsync(
                dbContext,
                baseQuery,
                It.Is<SearchOrchestratorContext>(ctx =>
                    ctx.SearchColumn == "name" &&
                    ctx.SearchTerm == "academy" &&
                    ctx.PageSize == 20 &&
                    ctx.Offset == 40 &&
                    ctx.Filters == filters),
                " AND type = 'Academy'",
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task GetMatchingIdsAsync_UsesEmptyFilterClause_WhenNoFiltersProvided()
    {
        // arrange
        EducationProviderRegistryDbContext dbContext =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        Mock<IDbContextFactory<EducationProviderRegistryDbContext>> factory =
            IDbContextFactoryTestDouble.MockFor(dbContext);

        IQueryable<Establishment> baseQuery =
            new List<Establishment>().AsQueryable();

        Mock<ISearchProjectionBuilder<Establishment>> projectionBuilder =
            SearchProjectionBuilderTestDouble.MockFor(dbContext, baseQuery);

        Mock<ISearchOrchestrator<Establishment>> orchestrator =
            SearchOrchestratorTestDouble.Mock();

        Mock<ISearchFilterExpressionsBuilder> filterBuilder =
            SearchFilterExpressionsBuilderTestDouble.MockFor(string.Empty);

        EstablishmentsSearchProvider provider =
            new(
                factory.Object,
                orchestrator.Object,
                projectionBuilder.Object,
                filterBuilder.Object,
                "urn");

        orchestrator.Setup(o => o.ExecuteAsync(
                dbContext,
                baseQuery,
                It.IsAny<SearchOrchestratorContext>(),
                string.Empty,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // act
        IReadOnlyList<Establishment> results =
            await provider.GetMatchingIdsAsync(
                "10001",
                10,
                0,
                [],
                CancellationToken.None);

        // assert
        Assert.Empty(results);

        orchestrator.Verify(o => o.ExecuteAsync(
            dbContext,
            baseQuery,
            It.Is<SearchOrchestratorContext>(ctx =>
                ctx.SearchColumn == "urn" &&
                ctx.SearchTerm == "10001"),
            string.Empty,
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetMatchingIdsAsync_CreatesDbContext_FromFactory()
    {
        // arrange
        EducationProviderRegistryDbContext dbContext =
            EducationProviderRegistryDbContextFactory.CreateDbContext();

        Mock<IDbContextFactory<EducationProviderRegistryDbContext>> factory =
            IDbContextFactoryTestDouble.MockFor(dbContext);

        Mock<ISearchProjectionBuilder<Establishment>> projectionBuilder =
            SearchProjectionBuilderTestDouble.MockFor(
                dbContext, new List<Establishment>().AsQueryable());

        Mock<ISearchOrchestrator<Establishment>> orchestrator =
            SearchOrchestratorTestDouble.Mock();

        orchestrator.Setup(o => o.ExecuteAsync(
                dbContext,
                It.IsAny<IQueryable<Establishment>>(),
                It.IsAny<SearchOrchestratorContext>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        Mock<ISearchFilterExpressionsBuilder> filterBuilder =
            SearchFilterExpressionsBuilderTestDouble.MockFor(string.Empty);

        EstablishmentsSearchProvider provider =
            new(
                factory.Object,
                orchestrator.Object,
                projectionBuilder.Object,
                filterBuilder.Object,
                "name");

        // act
        await provider.GetMatchingIdsAsync(
            "test",
            10,
            0,
            [],
            CancellationToken.None);

        // assert
        factory.Verify(dbContextFactory =>
            dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
