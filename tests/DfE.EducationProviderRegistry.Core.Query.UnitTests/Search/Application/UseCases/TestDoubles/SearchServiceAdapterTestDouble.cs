using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
internal sealed class SearchServiceAdapterTestDouble
{
    private readonly Mock<ISearchServiceAdapter<SearchAggregateResults, SearchFacets>> _mock;

    public SearchServiceAdapterTestDouble()
    {
        _mock = new Mock<ISearchServiceAdapter<
            SearchAggregateResults, SearchFacets>>(MockBehavior.Strict);
    }

    public SearchServiceAdapterRequest? CapturedRequest { get; private set; }

    public Mock<ISearchServiceAdapter<SearchAggregateResults, SearchFacets>> Returning(
        SearchResults<SearchAggregateResults, SearchFacets> results)
    {
        _mock
            .Setup(adapter =>
                adapter.SearchAsync(
                    It.IsAny<SearchServiceAdapterRequest>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(results);

        return _mock;
    }

    public Mock<ISearchServiceAdapter<SearchAggregateResults, SearchFacets>> Throwing(Exception exception)
    {
        _mock
            .Setup(adapter =>
                adapter.SearchAsync(
                    It.IsAny<SearchServiceAdapterRequest>(),
                    It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        return _mock;
    }

    public Mock<ISearchServiceAdapter<SearchAggregateResults, SearchFacets>> CapturingAndReturning(
        SearchResults<SearchAggregateResults, SearchFacets> results)
    {
        _mock
            .Setup(adapter =>
                adapter.SearchAsync(
                    It.IsAny<SearchServiceAdapterRequest>(),
                    It.IsAny<CancellationToken>()))
            .Callback<SearchServiceAdapterRequest, CancellationToken>((req, _) =>
            {
                CapturedRequest = req;
            })
            .ReturnsAsync(results);

        return _mock;
    }
}
