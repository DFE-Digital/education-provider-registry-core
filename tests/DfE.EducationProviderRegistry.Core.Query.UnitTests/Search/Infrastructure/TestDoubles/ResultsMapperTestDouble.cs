using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

public static class ResultsMapperTestDouble
{
    public static Mock<IMapper<(
        IReadOnlyList<SearchReadModel>,
        IReadOnlyList<AggregatedFacetResult>,
        int),
        SearchResults<SearchAggregateResults, SearchFacets>>> Mock()
    {
        Mock<IMapper<
            (
                IReadOnlyList<SearchReadModel>,
                IReadOnlyList<AggregatedFacetResult>,
                int
            ),
            SearchResults<SearchAggregateResults, SearchFacets>>> mock = new(MockBehavior.Strict);

        mock.Setup(mapper =>
            mapper.Map(
                It.IsAny<(
                    IReadOnlyList<SearchReadModel>,
                    IReadOnlyList<AggregatedFacetResult>,
                    int)>()))
            .Returns(new SearchResults<SearchAggregateResults, SearchFacets>());

        return mock;
    }
}
