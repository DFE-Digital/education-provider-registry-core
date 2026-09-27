using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;
using Microsoft.Extensions.Logging;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases;

public sealed class SearchUseCaseTests
{
    private readonly SearchCriteria _criteria;
    private readonly SearchResults<SearchAggregateResults, SearchFacets> _results;
    private readonly Mock<ILogger<SearchUseCase>> _logger;

    public SearchUseCaseTests()
    {
        _criteria = SearchCriteriaTestDouble.Stub();
        _results = SearchResultsTestDouble.Stub();
        _logger = MockTestDouble.Default<ILogger<SearchUseCase>>(MockBehavior.Loose);
    }

    private static SearchUseCase CreateSut(
        Mock<ILogger<SearchUseCase>> loggerMock,
        SearchCriteria criteria,
        ISearchServiceAdapter<SearchAggregateResults, SearchFacets> adapter) =>
            new(loggerMock.Object, criteria, adapter);

    [Fact]
    public async Task HandleRequest_ValidRequest_MapsParametersCorrectly()
    {
        // arrange
        SearchServiceAdapterTestDouble adapterDouble = new();
        Mock<ISearchServiceAdapter<SearchAggregateResults, SearchFacets>> adapter =
            adapterDouble.CapturingAndReturning(_results);

        SearchRequest request =
            new(
                [new SearchTerm(Key: "searchKey", Value: "searchkeyword")],
                new[] { FilterRequestTestDouble.Fake() },
                SortOrderTestDouble.Stub());

        SearchUseCase sut =
            CreateSut(_logger, _criteria, adapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await sut.HandleRequestAsync(request, TestContext.Current.CancellationToken);

        // assert
        adapter.Verify(adapter =>
            adapter.SearchAsync(
                It.IsAny<SearchServiceAdapterRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _logger.VerifyNoErrors();

        Assert.NotNull(adapterDouble.CapturedRequest);
        Assert.Equal(request.SearchTerms, adapterDouble.CapturedRequest!.SearchTerms);
        Assert.Equal(_criteria.SearchFields, adapterDouble.CapturedRequest.SearchFields);
        Assert.Equal(_criteria.Facets, adapterDouble.CapturedRequest.Facets);
        Assert.Equal(request.FilterRequests, adapterDouble.CapturedRequest.SearchFilterRequests);
    }

    [Fact]
    public async Task HandleRequest_ValidRequest_ReturnsSuccess()
    {
        // arrange
        SearchServiceAdapterTestDouble adapterDouble = new();
        Mock<ISearchServiceAdapter<SearchAggregateResults, SearchFacets>> adapter =
            adapterDouble.Returning(_results);

        SearchRequest request =
            new([new SearchTerm(Key: "searchKey", Value: "searchkeyword")],
            SortOrderTestDouble.Stub());

        SearchUseCase sut =
            CreateSut(_logger, _criteria, adapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await sut.HandleRequestAsync(request, TestContext.Current.CancellationToken);

        // assert
        adapter.Verify(adapter =>
            adapter.SearchAsync(
                It.IsAny<SearchServiceAdapterRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.NotNull(response.Model);
        Assert.Equal(SearchResponseStatus.Success, response.Model.Status);

        HashSet<SearchAggregateResult> expected = [.. _results.Results!.SearchResultCollection];
        HashSet<SearchAggregateResult> actual = [.. response.Model.SearchProviderResults!.SearchResultCollection];

        Assert.Subset(expected, actual);

        HashSet<SearchFacet> expectedFacets = [.. _results.FacetResults!.Facets];
        HashSet<SearchFacet> actualFacets = [.. response.Model.FacetedResults!.Facets];

        Assert.Subset(expectedFacets, actualFacets);
    }

    [Fact]
    public async Task HandleRequest_NullRequest_Throws()
    {
        // arrange
        SearchServiceAdapterTestDouble adapterDouble = new();
        Mock<ISearchServiceAdapter<SearchAggregateResults, SearchFacets>> adapter =
            adapterDouble.Returning(_results);

        SearchUseCase sut =
            CreateSut(_logger, _criteria, adapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await sut.HandleRequestAsync(
                request: null!,
                cancellationToken: TestContext.Current.CancellationToken);

        // verify
        adapter.Verify(adapter =>
            adapter.SearchAsync(
                It.IsAny<SearchServiceAdapterRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _logger.VerifyErrorContains("unexpected error");
        _logger.VerifyErrorContains("An unexpected error occurred while processing the search request.");

        // assert
        Assert.Null(response.Model);
        Assert.False(response.SuccessfulRequest);
        Assert.Equal(
            "An unexpected error occurred while processing the search request.",
            response.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequest_AdapterThrowsException_ReturnsError()
    {
        // arrange
        SearchServiceAdapterTestDouble adapterDouble = new();
        Mock<ISearchServiceAdapter<SearchAggregateResults, SearchFacets>> adapter =
            adapterDouble.Throwing(new ApplicationException());

        SearchRequest request =
            new(
                [new SearchTerm(Key: "searchKey", Value: "searchkeyword")],
                SortOrderTestDouble.Stub());

        SearchUseCase sut =
            CreateSut(_logger, _criteria, adapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await sut.HandleRequestAsync(request, TestContext.Current.CancellationToken);

        // verify
        _logger.VerifyErrorContains("unexpected error");
        _logger.VerifyErrorContains("An unexpected error occurred while processing the search request.");

        // assert
        Assert.Null(response.Model);
        Assert.False(response.SuccessfulRequest);
        Assert.Equal("An unexpected error occurred while processing the search request.", response.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequest_NoResults_ReturnsSuccessWithEmptyCollections()
    {
        // arrange
        SearchResults<SearchAggregateResults, SearchFacets> empty =
            SearchResultsTestDouble.StubWithNoResults();

        SearchServiceAdapterTestDouble adapterDouble = new();
        Mock<ISearchServiceAdapter<SearchAggregateResults, SearchFacets>> adapter =
            adapterDouble.Returning(empty);

        SearchRequest request =
            new(
                [new SearchTerm(Key: "searchKey", Value: "searchkeyword")],
                SortOrderTestDouble.Stub());

        SearchUseCase sut =
            CreateSut(_logger, _criteria, adapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await sut.HandleRequestAsync(request, TestContext.Current.CancellationToken);

        // verify
        _logger.VerifyNoErrors();

        // assert
        Assert.NotNull(response.Model);
        Assert.Equal(SearchResponseStatus.NoResultsFound, response.Model.Status);
        Assert.Empty(response.Model.SearchProviderResults!.SearchResultCollection);
    }

    [Fact]
    public async Task HandleRequest_AdapterThrowsSearchException_ReturnsDomainSpecificError()
    {
        // arrange
        SearchServiceAdapterTestDouble adapterDouble = new();
        Mock<ISearchServiceAdapter<SearchAggregateResults, SearchFacets>> adapter =
            adapterDouble.Throwing(new SearchException("boom"));

        SearchRequest request =
            new(
                [new SearchTerm(Key: "searchKey", Value: "searchkeyword")],
                SortOrderTestDouble.Stub());

        SearchUseCase sut =
            CreateSut(_logger, _criteria, adapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await sut.HandleRequestAsync(request, TestContext.Current.CancellationToken);

        // verify
        _logger.VerifyErrorContains("SearchUseCase domain-specific error");
        _logger.VerifyErrorContains("A domain-specific error occurred during search.");

        // assert
        Assert.Null(response.Model);
        Assert.False(response.SuccessfulRequest);
        Assert.Equal("A domain-specific error occurred during search.", response.ErrorMessage);
    }

    [Fact]
    public async Task HandleRequest_AdapterThrowsOperationCanceledException_ReturnsCancelledError()
    {
        // arrange
        SearchServiceAdapterTestDouble adapterDouble = new();
        Mock<ISearchServiceAdapter<SearchAggregateResults, SearchFacets>> adapter =
            adapterDouble.Throwing(new OperationCanceledException());

        SearchRequest request =
            new(
                [new SearchTerm(Key: "searchKey", Value: "searchkeyword")],
                SortOrderTestDouble.Stub());

        SearchUseCase sut =
            CreateSut(_logger, _criteria, adapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await sut.HandleRequestAsync(request, TestContext.Current.CancellationToken);

        // verify
        _logger.VerifyWarningContains("SearchUseCase execution cancelled");
        _logger.VerifyWarningContains("The search request was cancelled by the caller.");

        // assert
        Assert.Null(response.Model);
        Assert.False(response.SuccessfulRequest);
        Assert.Equal("The search request was cancelled by the caller.", response.ErrorMessage);
    }
}
