using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Facets;

public class FacetAggregationStepTests
{
    [Fact]
    public async Task CalculateFacetsAsync_ReturnsEmpty_WhenFacetContextsIsEmpty()
    {
        // arrange
        IQueryable<SearchAggregate> matchingQuery =
            Array.Empty<SearchAggregate>().AsQueryable();

        IReadOnlyCollection<FacetContext> facetContexts = [];

        Mock<IFacetProvider> facetProviderMock = new();

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregator(facetProviderMock.Object)
                .CalculateFacetsAsync(
                    matchingQuery,
                    facetContexts,
                    CancellationToken.None);

        // assert
        Assert.Empty(result);

        facetProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateFacetsAsync_PassesFilterValuesForFacet_ToProvider()
    {
        // arrange
        IQueryable<SearchAggregate> matchingQuery =
            Array.Empty<SearchAggregate>().AsQueryable();

        IList<object> filterValues =
        [
            "MLD - Moderate Learning Difficulty",
            "SEMH - Social, Emotional and Mental Health"
        ];

        FilterRequest filterRequest = new("sen", filterValues);

        IReadOnlyCollection<FacetContext> facetContexts =
        [
            new("sen", filterRequest)
        ];

        Mock<IFacetProvider> facetProviderMock = new();

        facetProviderMock
            .Setup(facetProvider =>
                facetProvider.GetFacetsAsync(
                    matchingQuery,
                    "sen",
                    It.Is<IList<object>>(values => values.SequenceEqual(filterValues)),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregator(facetProviderMock.Object)
                .CalculateFacetsAsync(
                    matchingQuery,
                    facetContexts,
                    CancellationToken.None);

        // assert
        Assert.Single(result);
        Assert.Equal("sen", result[0].FacetName);

        facetProviderMock.Verify(
            facetProvider =>
                facetProvider.GetFacetsAsync(
                    matchingQuery,
                    "sen",
                    It.Is<IList<object>>(values => values.SequenceEqual(filterValues)),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        facetProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateFacetsAsync_CallsProviderOncePerFacet()
    {
        // arrange
        IQueryable<SearchAggregate> matchingQuery =
            Array.Empty<SearchAggregate>().AsQueryable();

        IReadOnlyCollection<FacetContext> facetContexts =
        [
            new("Type", null)
        ];

        Mock<IFacetProvider> facetProviderMock = new();

        facetProviderMock
            .Setup(facetProvider =>
                facetProvider.GetFacetsAsync(
                    matchingQuery,
                    "Type",
                    It.Is<IList<object>>(values => values.Count == 0),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregator(facetProviderMock.Object)
                .CalculateFacetsAsync(
                    matchingQuery,
                    facetContexts,
                    CancellationToken.None);

        // assert
        Assert.Single(result);
        Assert.Equal("Type", result[0].FacetName);

        facetProviderMock.Verify(
            facetProvider =>
                facetProvider.GetFacetsAsync(
                    matchingQuery,
                    "Type",
                    It.Is<IList<object>>(values => values.Count == 0),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        facetProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateFacetsAsync_ReturnsAggregatedResults_FromProvider()
    {
        // arrange
        IQueryable<SearchAggregate> matchingQuery =
            Array.Empty<SearchAggregate>().AsQueryable();

        IReadOnlyCollection<FacetContext> facetContexts =
        [
            new("Status", null)
        ];

        IReadOnlyList<FacetResult> providerResults =
            new List<FacetResult>
            {
                new("Open", "label", 10),
                new("Closed", "label", 5)
            };

        Mock<IFacetProvider> facetProviderMock = new();

        facetProviderMock
            .Setup(facetProvider =>
                facetProvider.GetFacetsAsync(
                    matchingQuery,
                    "Status",
                    It.Is<IList<object>>(values => values.Count == 0),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(providerResults);

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregator(facetProviderMock.Object)
                .CalculateFacetsAsync(
                    matchingQuery,
                    facetContexts,
                    CancellationToken.None);

        // assert
        Assert.Single(result);
        Assert.Equal("Status", result[0].FacetName);
        Assert.Equal(providerResults, result[0].Values);

        facetProviderMock.VerifyAll();
    }

    [Fact]
    public async Task CalculateFacetsAsync_PreservesFacetOrder()
    {
        // arrange
        IQueryable<SearchAggregate> matchingQuery =
            Array.Empty<SearchAggregate>().AsQueryable();

        IReadOnlyCollection<FacetContext> facetContexts =
        [
            new("A", null),
            new("B", null),
            new("C", null)
        ];

        Mock<IFacetProvider> facetProviderMock = new();

        facetProviderMock
            .Setup(facetProvider =>
                facetProvider.GetFacetsAsync(
                    matchingQuery,
                    "A",
                    It.Is<IList<object>>(values => values.Count == 0),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        facetProviderMock
            .Setup(facetProvider =>
                facetProvider.GetFacetsAsync(
                    matchingQuery,
                    "B",
                    It.Is<IList<object>>(values => values.Count == 0),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        facetProviderMock
            .Setup(facetProvider =>
                facetProvider.GetFacetsAsync(
                    matchingQuery,
                    "C",
                    It.Is<IList<object>>(values => values.Count == 0),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregator(facetProviderMock.Object)
                .CalculateFacetsAsync(
                    matchingQuery,
                    facetContexts,
                    CancellationToken.None);

        // assert
        Assert.Equal(
            ["A", "B", "C"],
            result.Select(result => result.FacetName));

        facetProviderMock.VerifyAll();
    }

    [Fact]
    public async Task CalculateFacetsAsync_PropagatesExceptions_FromProvider()
    {
        // arrange
        IQueryable<SearchAggregate> matchingQuery =
            Array.Empty<SearchAggregate>().AsQueryable();

        IReadOnlyCollection<FacetContext> facetContexts =
        [
            new("ErrorFacet", null)
        ];

        Mock<IFacetProvider> facetProviderMock = new();

        facetProviderMock
            .Setup(facetProvider =>
                facetProvider.GetFacetsAsync(
                    matchingQuery,
                    "ErrorFacet",
                    It.Is<IList<object>>(values => values.Count == 0),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Boom"));

        // act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new FacetAggregator(facetProviderMock.Object)
                .CalculateFacetsAsync(
                    matchingQuery,
                    facetContexts,
                    CancellationToken.None));

        // assert
        facetProviderMock.VerifyAll();
    }
}
