using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

public static class FacetAggregatorTestDouble
{
    public static Mock<IFacetAggregator> Mock()
    {
        Mock<IFacetAggregator> mock = new(MockBehavior.Strict);

        mock.Setup(facetAggregator =>
            facetAggregator.CalculateFacetsAsync(
                It.IsAny<IReadOnlyList<string>>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AggregatedFacetResult>());

        return mock;
    }
}

