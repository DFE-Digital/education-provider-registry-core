using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

public static class ResultsMapperTestDouble
{
    public static Mock<IMapper<(
        IReadOnlyList<EstablishmentReadModel>,
        IReadOnlyList<AggregatedFacetResult>,
        int),
        SearchResults<EstablishmentSearchResults, SearchFacets>>> Mock()
    {
        Mock<IMapper<
            (
                IReadOnlyList<EstablishmentReadModel>,
                IReadOnlyList<AggregatedFacetResult>,
                int
            ),
            SearchResults<EstablishmentSearchResults, SearchFacets>>> mock = new(MockBehavior.Strict);

        mock.Setup(mapper =>
            mapper.Map(
                It.IsAny<(
                    IReadOnlyList<EstablishmentReadModel>,
                    IReadOnlyList<AggregatedFacetResult>,
                    int)>()))
            .Returns(new SearchResults<EstablishmentSearchResults, SearchFacets>());

        return mock;
    }
}
