using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

public static class FacetAggregatorTestDouble
{
    public static Mock<IFacetAggregator> Mock()
    {
        Mock<IFacetAggregator> mock = new(MockBehavior.Strict);

        mock.Setup(facetAggregator =>
            facetAggregator.CalculateFacetsAsync(
                It.IsAny<IQueryable<SearchAggregate>>(),
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AggregatedFacetResult>());

        return mock;
    }
}

