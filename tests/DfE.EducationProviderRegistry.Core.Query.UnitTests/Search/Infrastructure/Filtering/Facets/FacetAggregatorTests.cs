using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets.Providers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Facets;

public class FacetAggregatorTests
{
    [Fact]
    public async Task CalculateFacetsAsync_ReturnsEmpty_WhenFacetContextsIsEmpty()
    {
        // arrange
        IQueryable<SearchAggregate> matchingQuery =
            Array.Empty<SearchAggregate>().AsQueryable();

        IReadOnlyCollection<FacetFilterContext> facetContexts = [];

        IReadOnlyDictionary<string, IFacetResultProvider> facetResultProviders =
            new Dictionary<string, IFacetResultProvider>(StringComparer.OrdinalIgnoreCase);

        FacetAggregator sut =
            new(facetResultProviders);

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await sut.CalculateFacetsAsync(
                matchingQuery,
                facetContexts,
                CancellationToken.None);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CalculateFacetsAsync_PassesFilterValuesForFacet_ToProvider()
    {
        // arrange
        IList<object> filterValues =
        [
            "MLD - Moderate Learning Difficulty",
            "SEMH - Social, Emotional and Mental Health"
        ];

        FilterRequest filterRequest =
            new("sen", filterValues);

        IReadOnlyCollection<FacetFilterContext> facetContexts =
        [
            new("sen", filterRequest)
        ];

        SearchAggregate searchAggregate =
            new()
            {
                ProviderId = "1"
            };

        IQueryable<SearchAggregate> matchingQuery =
            CreateMatchingQuery(searchAggregate);

        Mock<IFacetResultProvider> facetResultProviderMock =
            new();

        facetResultProviderMock
            .Setup(provider =>
                provider.GetFacets(
                    It.IsAny<IEnumerable<FacetProjection>>(),
                    It.Is<IList<object>>(values =>
                        values.SequenceEqual(filterValues))))
            .Returns([]);

        IReadOnlyDictionary<string, IFacetResultProvider> facetResultProviders =
            new Dictionary<string, IFacetResultProvider>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["sen"] = facetResultProviderMock.Object
            };

        FacetAggregator sut =
            new(facetResultProviders);

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await sut.CalculateFacetsAsync(
                matchingQuery,
                facetContexts,
                CancellationToken.None);

        // assert
        AggregatedFacetResult aggregatedFacetResult =
            Assert.Single(result);

        Assert.Equal(
            "sen",
            aggregatedFacetResult.FacetName);

        facetResultProviderMock.Verify(
            provider =>
                provider.GetFacets(
                    It.IsAny<IEnumerable<FacetProjection>>(),
                    It.Is<IList<object>>(values =>
                        values.SequenceEqual(filterValues))),
            Times.Once);

        facetResultProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateFacetsAsync_CallsRegisteredProviderForFacet()
    {
        // arrange
        IReadOnlyCollection<FacetFilterContext> facetContexts =
        [
            new("boarding", null)
        ];

        SearchAggregate searchAggregate =
            new()
            {
                ProviderId = "1",
                Boarding = "Does not have boarders"
            };

        IQueryable<SearchAggregate> matchingQuery =
            CreateMatchingQuery(searchAggregate);

        Mock<IFacetResultProvider> facetResultProviderMock =
            new();

        facetResultProviderMock
            .Setup(provider =>
                provider.GetFacets(
                    It.IsAny<IEnumerable<FacetProjection>>(),
                    It.Is<IList<object>>(values =>
                        values.Count == 0)))
            .Returns([]);

        IReadOnlyDictionary<string, IFacetResultProvider> facetResultProviders =
            new Dictionary<string, IFacetResultProvider>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["boarding"] = facetResultProviderMock.Object
            };

        FacetAggregator sut =
            new(facetResultProviders);

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await sut.CalculateFacetsAsync(
                matchingQuery,
                facetContexts,
                CancellationToken.None);

        // assert
        AggregatedFacetResult aggregatedFacetResult =
            Assert.Single(result);

        Assert.Equal(
            "boarding",
            aggregatedFacetResult.FacetName);

        facetResultProviderMock.Verify(
            provider =>
                provider.GetFacets(
                    It.IsAny<IEnumerable<FacetProjection>>(),
                    It.Is<IList<object>>(values =>
                        values.Count == 0)),
            Times.Once);

        facetResultProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateFacetsAsync_ReturnsAggregatedResults_FromProvider()
    {
        // arrange
        IReadOnlyCollection<FacetFilterContext> facetContexts =
        [
            new("status", null)
        ];

        SearchAggregate searchAggregate =
            new()
            {
                ProviderId = "1",
                StatusName = "Open"
            };

        IQueryable<SearchAggregate> matchingQuery =
            CreateMatchingQuery(searchAggregate);

        IReadOnlyList<FacetResult> providerResults =
        [
            new("Open", "Open", 10),
            new("Closed", "Closed", 5)
        ];

        Mock<IFacetResultProvider> facetResultProviderMock =
            new();

        facetResultProviderMock
            .Setup(provider =>
                provider.GetFacets(
                    It.IsAny<IEnumerable<FacetProjection>>(),
                    It.Is<IList<object>>(values =>
                        values.Count == 0)))
            .Returns(providerResults);

        IReadOnlyDictionary<string, IFacetResultProvider> facetResultProviders =
            new Dictionary<string, IFacetResultProvider>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["status"] = facetResultProviderMock.Object
            };

        FacetAggregator sut =
            new(facetResultProviders);

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await sut.CalculateFacetsAsync(
                matchingQuery,
                facetContexts,
                CancellationToken.None);

        // assert
        AggregatedFacetResult aggregatedFacetResult =
            Assert.Single(result);

        Assert.Equal(
            "status",
            aggregatedFacetResult.FacetName);

        Assert.Equal(
            providerResults,
            aggregatedFacetResult.Values);

        facetResultProviderMock.VerifyAll();
    }

    [Fact]
    public async Task CalculateFacetsAsync_PreservesFacetOrder()
    {
        // arrange
        IReadOnlyCollection<FacetFilterContext> facetContexts =
        [
            new("boarding", null),
            new("status", null),
            new("sen", null)
        ];

        SearchAggregate searchAggregate =
            new()
            {
                ProviderId = "1"
            };

        IQueryable<SearchAggregate> matchingQuery =
            CreateMatchingQuery(searchAggregate);

        Mock<IFacetResultProvider> boardingProviderMock =
            CreateFacetResultProviderMock();

        Mock<IFacetResultProvider> statusProviderMock =
            CreateFacetResultProviderMock();

        Mock<IFacetResultProvider> senProviderMock =
            CreateFacetResultProviderMock();

        IReadOnlyDictionary<string, IFacetResultProvider> facetResultProviders =
            new Dictionary<string, IFacetResultProvider>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["boarding"] = boardingProviderMock.Object,
                ["status"] = statusProviderMock.Object,
                ["sen"] = senProviderMock.Object
            };

        FacetAggregator sut =
            new(facetResultProviders);

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await sut.CalculateFacetsAsync(
                matchingQuery,
                facetContexts,
                CancellationToken.None);

        // assert
        Assert.Equal(
            ["boarding", "status", "sen"],
            result.Select(aggregatedFacet =>
                aggregatedFacet.FacetName));

        boardingProviderMock.VerifyAll();
        statusProviderMock.VerifyAll();
        senProviderMock.VerifyAll();
    }

    [Fact]
    public async Task CalculateFacetsAsync_ThrowsInvalidOperationException_WhenFacetIsUnknown()
    {
        // arrange
        IReadOnlyCollection<FacetFilterContext> facetContexts =
        [
            new("unknown", null)
        ];

        SearchAggregate searchAggregate =
            new()
            {
                ProviderId = "1"
            };

        IQueryable<SearchAggregate> matchingQuery =
            CreateMatchingQuery(searchAggregate);

        IReadOnlyDictionary<string, IFacetResultProvider> facetResultProviders =
            new Dictionary<string, IFacetResultProvider>(
                StringComparer.OrdinalIgnoreCase);

        FacetAggregator sut =
            new(facetResultProviders);

        // act
        Task act() =>
            sut.CalculateFacetsAsync(
                matchingQuery,
                facetContexts,
                CancellationToken.None);

        // assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task CalculateFacetsAsync_PropagatesExceptions_FromProvider()
    {
        // arrange
        IReadOnlyCollection<FacetFilterContext> facetContexts =
        [
            new("status", null)
        ];

        SearchAggregate searchAggregate =
            new()
            {
                ProviderId = "1"
            };

        IQueryable<SearchAggregate> matchingQuery =
            CreateMatchingQuery(searchAggregate);

        Mock<IFacetResultProvider> facetResultProviderMock =
            new();

        facetResultProviderMock
            .Setup(provider =>
                provider.GetFacets(
                    It.IsAny<IEnumerable<FacetProjection>>(),
                    It.IsAny<IList<object>>()))
            .Throws(
                new InvalidOperationException("Boom"));

        IReadOnlyDictionary<string, IFacetResultProvider> facetResultProviders =
            new Dictionary<string, IFacetResultProvider>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["status"] = facetResultProviderMock.Object
            };

        FacetAggregator sut =
            new(facetResultProviders);

        // act
        Task act() =>
            sut.CalculateFacetsAsync(
                matchingQuery,
                facetContexts,
                CancellationToken.None);

        // assert
        await Assert.ThrowsAsync<InvalidOperationException>(act);

        facetResultProviderMock.VerifyAll();
    }

    private static IQueryable<SearchAggregate> CreateMatchingQuery(
        params SearchAggregate[] searchAggregates)
    {
        DbContextOptions<FacetTestDbContext> options =
            new DbContextOptionsBuilder<FacetTestDbContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString())
                .Options;

        FacetTestDbContext context =
            new(options);

        context.SearchAggregates.AddRange(
            searchAggregates);

        context.SaveChanges();

        return context.SearchAggregates;
    }

    private static Mock<IFacetResultProvider>
        CreateFacetResultProviderMock()
    {
        Mock<IFacetResultProvider> mock =
            new();

        mock
            .Setup(provider =>
                provider.GetFacets(
                    It.IsAny<IEnumerable<FacetProjection>>(),
                    It.Is<IList<object>>(values =>
                        values.Count == 0)))
            .Returns([]);

        return mock;
    }

    private sealed class FacetTestDbContext : DbContext
    {
        public FacetTestDbContext(
            DbContextOptions<FacetTestDbContext> options)
            : base(options)
        {
        }

        public DbSet<SearchAggregate> SearchAggregates =>
            Set<SearchAggregate>();
    }
}
