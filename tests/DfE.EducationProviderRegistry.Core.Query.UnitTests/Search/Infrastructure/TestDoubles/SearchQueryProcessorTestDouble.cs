using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

public static class SearchQueryProcessorTestDouble
{
    public static Mock<ISearchQueryProcessor<SearchAggregate>> Mock()
    {
        Mock<ISearchQueryProcessor<SearchAggregate>> mock = new(MockBehavior.Strict);

        mock.Setup(searchQueryProcessor =>
            searchQueryProcessor.ProcessSearch(
                It.IsAny<IQueryable<SearchAggregate>>(),
                It.IsAny<IEnumerable<SearchTerm?>?>()))
            .Returns((IQueryable<SearchAggregate> queryable, IEnumerable<SearchTerm?>? _) => queryable);

        return mock;
    }
}
