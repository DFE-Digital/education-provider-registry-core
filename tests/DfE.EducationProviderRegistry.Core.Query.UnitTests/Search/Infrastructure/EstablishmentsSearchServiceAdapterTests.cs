using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure;

public sealed class EstablishmentsSearchServiceAdapterTests
{
    [Fact]
    public async Task SearchAsync_ReturnsEmptyResults_WhenNoEstablishmentsFound()
    {
        // arrange
        Mock<ISearchProvider<Establishment>> searchProvider =
            SearchProviderTestDouble.MockFor([]);

        Mock<IMapper<
            SearchPipelineContext,
            SearchResults<EstablishmentSearchResults, SearchFacets>>> resultsMapper =
                SearchResultsMapperTestDouble.Mock();

        ReadOnlyCollection<SearchFilterRequest> emptyMappedFilters =
            new List<SearchFilterRequest>().AsReadOnly();

        Mock<IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>>> filterMapper =
                FilterResultsMapperTestDouble.MockFor(emptyMappedFilters);

        EstablishmentsSearchServiceAdapter adapter =
            new(
                searchProvider.Object,
                Mock.Of<IFacetProvider>(),
                [],
                resultsMapper.Object,
                filterMapper.Object);

        SearchServiceAdapterRequest request =
            new(
                searchIndexKey: "establishments",
                searchKeyword: "abc",
                searchFields: ["Name"],
                sortOrdering: new SortOrder("Name", "Asc", new List<string>() { "Name" }.AsReadOnly()),
                facets: [],
                searchFilterRequests: [],
                offset: 0);

        // act
        SearchResults<EstablishmentSearchResults, SearchFacets> result =
            await adapter.SearchAsync(
                request, TestContext.Current.CancellationToken);

        // assert
        Assert.Null(result.Results);
        Assert.Null(result.FacetResults);

        resultsMapper.Verify(resultsMapper =>
            resultsMapper.Map(It.IsAny<SearchPipelineContext>()), Times.Never);
    }

    [Fact]
    public async Task SearchAsync_ExecutesPipelineSteps_InOrder()
    {
        // arrange
        List<Establishment> establishments =
            [
                new Establishment { Urn = "111" }
            ];

        Mock<ISearchProvider<Establishment>> provider =
            SearchProviderTestDouble.MockFor(establishments);

        Mock<ISearchPipelineStep> step1 = new();
        Mock<ISearchPipelineStep> step2 = new();

        Mock<IMapper<
            SearchPipelineContext,
            SearchResults<EstablishmentSearchResults, SearchFacets>>> resultsMapper =
                SearchResultsMapperTestDouble.MockFor(
                    new SearchResults<EstablishmentSearchResults, SearchFacets>());

        ReadOnlyCollection<SearchFilterRequest> emptyMappedFilters =
            new List<SearchFilterRequest>().AsReadOnly();

        Mock<IMapper<ReadOnlyCollection<FilterRequest>, ReadOnlyCollection<SearchFilterRequest>>> filterMapper =
            FilterResultsMapperTestDouble.MockFor(emptyMappedFilters);

        EstablishmentsSearchServiceAdapter adapter =
            new(
                provider.Object,
                Mock.Of<IFacetProvider>(),
                [step1.Object, step2.Object],
                resultsMapper.Object,
                filterMapper.Object);

        SearchServiceAdapterRequest request =
            new(
                searchIndexKey: "establishments",
                searchKeyword: "abc",
                searchFields: ["Name"],
                sortOrdering: new SortOrder("Name", "Asc", new List<string>() { "Name" }.AsReadOnly()),
                facets: [],
                searchFilterRequests: [],
                offset: 0);

        // act
        await adapter.SearchAsync(request, TestContext.Current.CancellationToken);

        // assert
        step1.Verify(searchPipelineStep =>
            searchPipelineStep.Execute(
                It.IsAny<SearchPipelineContext>(),
                It.IsAny<CancellationToken>()), Times.Once);

        step2.Verify(searchPipelineStep =>
            searchPipelineStep.Execute(
                It.IsAny<SearchPipelineContext>(),
                It.IsAny<CancellationToken>()), Times.Once);

        step1.VerifyNoOtherCalls();
        step2.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SearchAsync_PassesCorrectParameters_ToSearchProvider()
    {
        // arrange
        Mock<ISearchProvider<Establishment>> provider =
            SearchProviderTestDouble.MockFor([new Establishment { Urn = "111" }]);

        Mock<IMapper<
            SearchPipelineContext,
            SearchResults<EstablishmentSearchResults, SearchFacets>>> resultsMapper =
                SearchResultsMapperTestDouble.MockFor(
                    new SearchResults<EstablishmentSearchResults, SearchFacets>());

        ReadOnlyCollection<SearchFilterRequest> mappedFilters = new List<SearchFilterRequest>().AsReadOnly();

        Mock<IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>>> filterMapper =
                FilterResultsMapperTestDouble.MockFor(mappedFilters);

        EstablishmentsSearchServiceAdapter adapter =
            new(
                provider.Object,
                Mock.Of<IFacetProvider>(),
                [],
                resultsMapper.Object,
                filterMapper.Object);

        SearchServiceAdapterRequest request =
            new(
                searchIndexKey: "establishments",
                searchKeyword: "hello",
                searchFields: ["Name"],
                sortOrdering: new SortOrder("Name", "Asc", new List<string>() { "Name" }.AsReadOnly()),
                facets: [],
                searchFilterRequests: [
                    new FilterRequest( filterName: "x", filterValues: [])
                ],
                offset: 0);

        // act
        await adapter.SearchAsync(request, TestContext.Current.CancellationToken);

        // assert
        provider.Verify(searchProvider =>
            searchProvider.GetMatchingIdsAsync(
                searchTerm: "hello",
                pageSize: 50,
                offset: 0,
                mappedFilters,
                It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact]
    public async Task SearchAsync_SetsContextCorrectly()
    {
        // arrange
        List<Establishment> establishments =
            [
                new Establishment { Urn = "111" },
                new Establishment { Urn = "222" }
            ];

        Mock<ISearchProvider<Establishment>> provider =
            SearchProviderTestDouble.MockFor(establishments);

        Mock<ISearchPipelineStep> step = new();

        SearchPipelineContext? capturedContext = null;

        step
            .Setup(s => s.Execute(It.IsAny<SearchPipelineContext>(), It.IsAny<CancellationToken>()))
            .Callback<SearchPipelineContext, CancellationToken>((ctx, _) => capturedContext = ctx);

        SearchResults<EstablishmentSearchResults, SearchFacets> mappedResults = new();

        Mock<IMapper<
            SearchPipelineContext,
            SearchResults<EstablishmentSearchResults, SearchFacets>>> resultsMapper =
                SearchResultsMapperTestDouble.MockFor(mappedResults);

        ReadOnlyCollection<SearchFilterRequest> mappedFilters =
            new List<SearchFilterRequest>().AsReadOnly();

        Mock<IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>>> filterMapper =
                FilterResultsMapperTestDouble.MockFor(mappedFilters);


        EstablishmentsSearchServiceAdapter adapter =
            new(
                provider.Object,
                Mock.Of<IFacetProvider>(),
                [step.Object],
                resultsMapper.Object,
                filterMapper.Object);

        SearchServiceAdapterRequest request =
            new(
                searchIndexKey: "establishments",
                searchKeyword: "abc",
                searchFields: ["Name"],
                sortOrdering: new SortOrder("Name", "Asc", new List<string>() { "Name" }.AsReadOnly()),
                facets: [],
                searchFilterRequests: [],
                offset: 0);

        // act
        await adapter.SearchAsync(request, TestContext.Current.CancellationToken);

        // assert
        ReadOnlyCollection<string?> urns = capturedContext!.Get<ReadOnlyCollection<string?>>();
        IReadOnlyList<Establishment> ests = capturedContext!.Get<IReadOnlyList<Establishment>>();
        IReadOnlyCollection<string> facetKeys = capturedContext.Get<List<string>>();

        Assert.Contains("111", urns);
        Assert.Contains("222", urns);
        Assert.Equal(2, ests.Count);
        Assert.Contains("EstablishmentTypeId", facetKeys);
    }
}
