using System.Diagnostics.CodeAnalysis;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchResultsMapperTestDouble
{
    public static Mock<IMapper<
        SearchPipelineContext,
        SearchResults<SearchProviderResults, SearchFacets>>> Mock() => new(MockBehavior.Strict);

    public static Mock<IMapper<
        SearchPipelineContext,
        SearchResults<SearchProviderResults, SearchFacets>>> MockFor(
        SearchResults<SearchProviderResults, SearchFacets> searchResults)
    {
        Mock<IMapper<SearchPipelineContext, SearchResults<SearchProviderResults, SearchFacets>>> searchResultsMapper = Mock();

        searchResultsMapper
            .Setup(mapper =>
                    mapper.Map(It.IsAny<SearchPipelineContext>()))
                .Returns(searchResults);

        return searchResultsMapper;
    }
}
