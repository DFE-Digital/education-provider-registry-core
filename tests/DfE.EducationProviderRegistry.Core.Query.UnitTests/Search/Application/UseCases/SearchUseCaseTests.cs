using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases;

public sealed class SearchUseCaseTests
{
    private readonly SearchResults<EstablishmentSearchResults, SearchFacets> _searchResults;
    private readonly SearchCriteria _searchCriteriaStub = SearchCriteriaTestDouble.Stub();
    private readonly Mock<ILogger<SearchUseCase>> _loggerMock;

    public SearchUseCaseTests()
    {
        // arrange
        _searchResults = SearchResultsTestDouble.Stub();
        _loggerMock = MockTestDouble.Default<ILogger<SearchUseCase>>(MockBehavior.Loose);
    }

    [Fact]
    public async Task HandleRequest_ValidRequest_CallsAdapterWithMappedRequestParams()
    {
        // arrange
        Mock<ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>> mockSearchServiceAdapter =
            new SearchServiceAdapterTestDouble().MockFor(_searchResults);

        SearchServiceAdapterRequest? adapterRequest = null;

        mockSearchServiceAdapter
            .Setup(adapter =>
                adapter.SearchAsync(
                    It.IsAny<SearchServiceAdapterRequest>(),
                    It.IsAny<CancellationToken>()))
            .Callback<SearchServiceAdapterRequest, CancellationToken>((req, token) =>
            {
                adapterRequest = req;
            })
            .ReturnsAsync(_searchResults);

        SearchRequest request =
            new(
                searchIndexKey: "stubIndexKey",
                searchKeywords: "searchkeyword",
                filterRequests: [FilterRequestTestDouble.Fake()],
                sortOrder: SortOrderTestDouble.Stub()
            );

        SearchUseCase useCase =
            new(_loggerMock.Object,
                _searchCriteriaStub,
                mockSearchServiceAdapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await useCase.HandleRequestAsync(
                request,
                TestContext.Current.CancellationToken);

        // verify
        mockSearchServiceAdapter.Verify(searchServiceAdapter =>
            searchServiceAdapter.SearchAsync(
                It.IsAny<SearchServiceAdapterRequest>(),
                It.IsAny<CancellationToken>()), Times.Once());

        // assert
        Assert.Equal(request.SearchKeywords, adapterRequest!.SearchKeyword);
        Assert.Equal(_searchCriteriaStub.SearchFields, adapterRequest.SearchFields);
        Assert.Equal(_searchCriteriaStub.Facets, adapterRequest.Facets);
        Assert.Equal(request.FilterRequests, adapterRequest.SearchFilterRequests);
    }

    [Fact]
    public async Task HandleRequest_ValidRequest_ReturnsResponse()
    {
        // arrange
        Mock<ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>> mockSearchServiceAdapter =
            new SearchServiceAdapterTestDouble().MockFor(_searchResults);

        SearchRequest request =
            new(
                searchIndexKey: "stubIndexKey",
                searchKeywords: "searchkeyword",
                sortOrder: SortOrderTestDouble.Stub());

        SearchUseCase useCase =
            new(
                _loggerMock.Object,
                _searchCriteriaStub,
                mockSearchServiceAdapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await useCase.HandleRequestAsync(
                request,
                TestContext.Current.CancellationToken);

        // verify
        mockSearchServiceAdapter.Verify(searchServiceAdapter =>
            searchServiceAdapter.SearchAsync(
                It.IsAny<SearchServiceAdapterRequest>(),
                It.IsAny<CancellationToken>()), Times.Once());

        // assert
        Assert.NotNull(response.Model);
        Assert.Equal(SearchResponseStatus.Success, response.Model.Status);
        Assert.NotNull(response.Model.EstablishmentResults);
        Assert.NotNull(response.Model.EstablishmentResults.EstablishmentCollection);
        Assert.NotNull(_searchResults.Results);
        Assert.Subset(
            new HashSet<EstablishmentSearchResult>(_searchResults.Results.EstablishmentCollection),
            new HashSet<EstablishmentSearchResult>(response.Model.EstablishmentResults.EstablishmentCollection));

        Assert.NotNull(response.Model.FacetedResults);
        Assert.NotNull(response.Model.FacetedResults.Facets);
        Assert.NotNull(_searchResults.FacetResults);
        Assert.NotNull(_searchResults.FacetResults.Facets);
        Assert.Subset(
            new HashSet<SearchFacet>(_searchResults.FacetResults.Facets),
            new HashSet<SearchFacet>(response.Model.FacetedResults.Facets));
    }

    [Fact]
    public async Task HandleRequest_NullSearchByKeywordRequest_ReturnsErrorStatus()
    {
        // arrange
        Mock<ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>> mockSearchServiceAdapter =
            new SearchServiceAdapterTestDouble().MockFor(_searchResults);

        SearchUseCase useCase =
            new(
                _loggerMock.Object,
                _searchCriteriaStub,
                mockSearchServiceAdapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await useCase.HandleRequestAsync(
                null!,
                TestContext.Current.CancellationToken);

        // verify
        mockSearchServiceAdapter.Verify(searchServiceAdapter =>
            searchServiceAdapter.SearchAsync(It.IsAny<SearchServiceAdapterRequest>()), Times.Never());

        // assert
        Assert.NotNull(response.Model);
        Assert.Equal(SearchResponseStatus.InvalidRequest, response.Model.Status);

        // TODO: ensure the correct logging occurs for invalid requests, possibly by using a logging test double or mock.

    }

    [Fact]
    public async Task HandleRequest_ServiceAdapterThrowsException_ReturnsErrorStatus()
    {
        // arrange
        Mock<ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>> mockSearchServiceAdapter =
            new SearchServiceAdapterTestDouble().MockFor(_searchResults);

        SearchRequest request =
            new(
                searchIndexKey: "stubIndexKey",
                searchKeywords: "searchkeyword",
                sortOrder: SortOrderTestDouble.Stub());

        Mock.Get(mockSearchServiceAdapter.Object)
            .Setup(adapter => adapter.SearchAsync(It.IsAny<SearchServiceAdapterRequest>()))
            .ThrowsAsync(new ApplicationException());

        SearchUseCase useCase =
            new(
                _loggerMock.Object,
                _searchCriteriaStub,
                mockSearchServiceAdapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await useCase.HandleRequestAsync(
                null!,
                TestContext.Current.CancellationToken);

        // verify
        mockSearchServiceAdapter.Verify(searchServiceAdapter =>
            searchServiceAdapter.SearchAsync(It.IsAny<SearchServiceAdapterRequest>()), Times.Once());

        // assert
        Assert.NotNull(response.Model);
        Assert.Equal(SearchResponseStatus.SearchServiceError, response.Model.Status);

        // TODO: ensure the correct logging occurs for invalid requests, possibly by using a logging test double or mock.
    }

    [Fact]
    public async Task HandleRequest_NoResults_ReturnsSuccess()
    {
        // arrange
        Mock<ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>> mockSearchServiceAdapter =
            new SearchServiceAdapterTestDouble().MockFor(_searchResults);

        SearchRequest request =
            new(
                searchIndexKey: "stubIndexKey",
                searchKeywords: "searchkeyword",
                sortOrder: SortOrderTestDouble.Stub());

        Mock.Get(mockSearchServiceAdapter.Object)
            .Setup(adapter =>
                adapter.SearchAsync(It.IsAny<SearchServiceAdapterRequest>()))
            .ReturnsAsync(SearchResultsTestDouble.StubWithNoResults);

        SearchUseCase useCase =
            new(
                _loggerMock.Object,
                _searchCriteriaStub,
                mockSearchServiceAdapter.Object);

        // act
        UseCaseResponse<SearchResponse> response =
            await useCase.HandleRequestAsync(
                null!,
                TestContext.Current.CancellationToken);

        // verify
        mockSearchServiceAdapter.Verify(searchServiceAdapter =>
            searchServiceAdapter.SearchAsync(It.IsAny<SearchServiceAdapterRequest>()), Times.Once());

        // assert
        Assert.NotNull(response.Model);
        Assert.Equal(SearchResponseStatus.NoResultsFound, response.Model.Status);

        // TODO: ensure the correct logging occurs for invalid requests, possibly by using a logging test double or mock.
    }
}
