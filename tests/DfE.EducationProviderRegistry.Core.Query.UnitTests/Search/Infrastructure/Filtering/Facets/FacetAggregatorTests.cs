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
        IReadOnlyList<string> urns = new[] { "100", "200" };

        Mock<IFacetProvider> facetProviderMock = FacetProviderTestDouble.Mock();

        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregator(facetProviderMock.Object)
                .CalculateFacetsAsync(urns, null, CancellationToken.None);

        Assert.Empty(result);
        facetProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateFacetsAsync_ReturnsEmpty_WhenRequestedFacetsIsEmpty()
    {
        IReadOnlyList<string> urns = new[] { "100", "200" };

        Mock<IFacetProvider> facetProviderMock = FacetProviderTestDouble.Mock();

        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregator(facetProviderMock.Object)
                .CalculateFacetsAsync(urns, [], CancellationToken.None);

        Assert.Empty(result);
        facetProviderMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CalculateFacetsAsync_CallsProviderOncePerDistinctFacet_IgnoringCase()
    {
        // arrange
        IReadOnlyList<string> urns = new[] { "100", "200" };
        IEnumerable<string> requested = new[] { "Type", "type", "TYPE" };

        Mock<IFacetProvider> facetProviderMock = FacetProviderTestDouble.Mock(builder =>
            builder.Returns("Type", new List<FacetResult>())
        );

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregator(facetProviderMock.Object)
                .CalculateFacetsAsync(urns, requested, CancellationToken.None);

        // assert/verify
        Assert.Single(result);
        Assert.Equal("Type", result[0].FacetName);

        facetProviderMock.Verify(
            facetProvider =>
                facetProvider.GetFacetsAsync(
                    urns, "Type", It.IsAny<CancellationToken>()),
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
                new("Open", "label", 10),
                new("Closed", "label", 5)
            };

        Mock<IFacetProvider> facetProviderMock =
            FacetProviderTestDouble.Mock(builder =>
                builder.Returns("Status", providerResults));

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregator(facetProviderMock.Object)
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

        Mock<IFacetProvider> facetProviderMock = FacetProviderTestDouble.Mock(builder =>
        {
            builder.Returns("A", new List<FacetResult>());
            builder.Returns("B", new List<FacetResult>());
            builder.Returns("C", new List<FacetResult>());
        });

        // act
        IReadOnlyList<AggregatedFacetResult> result =
            await new FacetAggregator(facetProviderMock.Object)
                .CalculateFacetsAsync(urns, requested, CancellationToken.None);

        // assert/verify
        Assert.Equal(["A", "B", "C"],
            result.Select(result => result.FacetName));

        facetProviderMock.VerifyAll();
    }

    [Fact]
    public async Task CalculateFacetsAsync_PropagatesExceptions_FromProvider()
    {
        // arrange
        IReadOnlyList<string> urns = new[] { "100", "200" };
        IEnumerable<string> requested = new[] { "ErrorFacet" };

        Mock<IFacetProvider> facetProviderMock =
            FacetProviderTestDouble.Mock(builder =>
                builder.Throws("ErrorFacet", new InvalidOperationException("Boom")));

        // act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            new FacetAggregator(facetProviderMock.Object)
                .CalculateFacetsAsync(urns, requested, CancellationToken.None));

        // verify
        facetProviderMock.VerifyAll();
    }
}
