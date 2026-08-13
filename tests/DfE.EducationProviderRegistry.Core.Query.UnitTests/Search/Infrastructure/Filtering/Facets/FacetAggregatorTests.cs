using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Facets.TestDoubles;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Facets;

public class FacetAggregationStepTests
{
    [Fact]
    public async Task CalculateFacetsAsync_ReturnsEmpty_WhenRequestedFacetsIsNull()
    {
        // arrange
        IReadOnlyList<string> urns = new[] { "100", "200" };

        Mock<IFacetProvider> facetProviderMock = FacetProviderTestDouble.Mock();

        // act
        IReadOnlyList <AggregatedFacetResult> result =
            await new FacetAggregationStep(facetProviderMock.Object)
                .CalculateFacetsAsync(urns, null, CancellationToken.None);

        // assert/verify
        Assert.Empty(result);
        facetProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateFacetsAsync_ReturnsEmpty_WhenRequestedFacetsIsEmpty()
    {
        // arrange
        IReadOnlyList<string> urns = new[] { "100", "200" };

        Mock<IFacetProvider> facetProviderMock = FacetProviderTestDouble.Mock();

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregationStep(facetProviderMock.Object)
                .CalculateFacetsAsync(urns, [], CancellationToken.None);

        // assert/verify
        Assert.Empty(result);
        facetProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateFacetsAsync_CallsProviderOncePerDistinctFacet_IgnoringCase()
    {
        // arrange
        IReadOnlyList<string> urns = new[] { "100", "200" };
        IEnumerable<string> requested = new[] { "Type", "type", "TYPE" };

        Mock<IFacetProvider> facetProviderMock =
            FacetProviderTestDouble
                .MockFor(new Dictionary<string, IReadOnlyList<FacetResult>>
                {
                    ["Type"] = new List<FacetResult>()
                });

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregationStep(facetProviderMock.Object)
                .CalculateFacetsAsync(urns, requested, CancellationToken.None);

        // assert/verify
        Assert.Single(result);
        Assert.Equal("Type", result[0].FacetName);

        facetProviderMock.Verify(
            facetProvider =>
                facetProvider.GetFacetsAsync(
                    It.IsAny<IReadOnlyList<string>>(),
                    "Type",
                    It.IsAny<CancellationToken>()),
                Times.Once);

        facetProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateFacetsAsync_ReturnsAggregatedResults_FromProvider()
    {
        // arrange
        IReadOnlyList<string> urns = new[] { "100", "200" };
        IEnumerable<string> requested = new[] { "Status" };

        IReadOnlyList<FacetResult> providerResults =
            new List<FacetResult>
            {
                new("Open", "status", 10),
                new("Closed", "status", 5)
            };

        Mock<IFacetProvider> facetProviderMock =
            FacetProviderTestDouble.MockFor(
                new Dictionary<string, IReadOnlyList<FacetResult>>
                {
                    ["Status"] = providerResults
                });

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregationStep(facetProviderMock.Object)
                .CalculateFacetsAsync(urns, requested, CancellationToken.None);

        // assert/verify
        Assert.Single(result);
        Assert.Equal("Status", result[0].FacetName);
        Assert.Equal(providerResults, result[0].Values);
        facetProviderMock.VerifyAll();
    }

    [Fact]
    public async Task CalculateFacetsAsync_PreservesFacetOrder()
    {
        // arrange
        IReadOnlyList<string> urns = new[] { "100", "200" };
        IEnumerable<string> requested = new[] { "A", "B", "C" };

        Mock<IFacetProvider> facetProviderMock =
            FacetProviderTestDouble.MockFor(
                new Dictionary<string, IReadOnlyList<FacetResult>>
                {
                    ["A"] = new List<FacetResult>(),
                    ["B"] = new List<FacetResult>(),
                    ["C"] = new List<FacetResult>()
                });

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregationStep(facetProviderMock.Object)
                .CalculateFacetsAsync(urns, requested, CancellationToken.None);

        // assert/verify
        Assert.Equal([ "A", "B", "C" ],
            result.Select(aggregatedFacetResult =>
                aggregatedFacetResult.FacetName));

        facetProviderMock.VerifyAll();
    }

    [Fact]
    public async Task CalculateFacetsAsync_PropagatesExceptions_FromProvider()
    {
        // arrange
        IReadOnlyList<string> urns = new[] { "100", "200" };
        IEnumerable<string> requested = new[] { "ErrorFacet" };

        Mock<IFacetProvider> facetProviderMock =
            FacetProviderTestDouble.MockFor(
                throwKey: "ErrorFacet",
                exception: new InvalidOperationException("Boom"));

        // act/assert/verify
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new FacetAggregationStep(facetProviderMock.Object)
                .CalculateFacetsAsync(urns, requested, CancellationToken.None));

        facetProviderMock.VerifyAll();
    }
}
