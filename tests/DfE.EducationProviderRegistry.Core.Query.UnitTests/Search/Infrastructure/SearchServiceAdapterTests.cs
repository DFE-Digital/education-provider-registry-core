using System.Collections.ObjectModel;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure;

public class SearchServiceAdapterTests
{
    private readonly Mock<ISearchFilterExpressionsBuilder<SearchAggregate>> _filterBuilderMock;
    private readonly Mock<IMapper<(IReadOnlyList<SearchReadModel>, IReadOnlyList<AggregatedFacetResult>, int),
        SearchResults<SearchAggregateResults, SearchFacets>>> _resultsMapperMock;
    private readonly Mock<IMapper<ReadOnlyCollection<FilterRequest>, ReadOnlyCollection<SearchFilterRequest>>> _filterMapperMock;
    private readonly SearchServiceAdapter _sut;
    private readonly EducationProviderRegistryDbContext _db;

    public SearchServiceAdapterTests()
    {
        DbContextOptions<EducationProviderRegistryDbContext> options =
            new DbContextOptionsBuilder<EducationProviderRegistryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        _db = new EducationProviderRegistryDbContext(options);

        Mock<ISearchQueryProcessor<SearchAggregate>> searchProcessorMock = SearchQueryProcessorTestDouble.Mock();
        Mock<IFacetAggregator> facetAggregatorMock = FacetAggregatorTestDouble.Mock();

        _filterBuilderMock = SearchFilterExpressionsBuilderTestDouble.Mock();
        _resultsMapperMock = ResultsMapperTestDouble.Mock();
        _filterMapperMock = FilterMapperTestDouble.Mock();

        _sut = new SearchServiceAdapter(
            _db,
            searchProcessorMock.Object,
            _filterBuilderMock.Object,
            facetAggregatorMock.Object,
            _resultsMapperMock.Object,
            _filterMapperMock.Object);
    }

    [Fact]
    public async Task SearchAsync_Throws_WhenRequestIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.SearchAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task SearchAsync_MapsFilterRequests()
    {
        // arrange
        SearchServiceAdapterRequest request = new(
            searchTerms: new List<SearchTerm>
            {
                new("key", "value")
            },
            searchFields: ["abc"],
            sortOrdering: new SortOrder("Name", "asc", new List<string> { "Name" }));

        ReadOnlyCollection<SearchFilterRequest> mapped = new(
            [new("key", ["value"])]);

        // act
        await _sut.SearchAsync(request, CancellationToken.None);

        // verify
        _filterMapperMock.Verify(mapper =>
            mapper.Map(It.IsAny<ReadOnlyCollection<FilterRequest>>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_AppliesFilterPredicate()
    {
        // arrange
        SearchServiceAdapterRequest request = new(
            searchTerms: new List<SearchTerm>
            {
                new("key", "value")
            },
            searchFields: ["abc"],
            sortOrdering: new SortOrder("Name", "asc", new List<string> { "Name" }));

        ReadOnlyCollection<SearchFilterRequest> mapped = new([]);

        // act
        await _sut.SearchAsync(request, CancellationToken.None);

        // verify
        _filterBuilderMock.Verify(searchFilterExpressionBuilder =>
            searchFilterExpressionBuilder.BuildSearchFilterExpression(mapped), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ProjectsEstablishmentsCorrectly()
    {
        // arrange
        _db.SearchAggregate.Add(new SearchAggregate
        {
            SearchAggregateId = 1,
            ProviderId = "100",
            ProviderName = "School A",
            ProviderAddress = "Addr, Town, County, PC",
            ProviderTypeName = "Type",
            ProviderTypeId = 12,
        });

        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        SearchServiceAdapterRequest request = new(
            searchTerms: new List<SearchTerm>
            {
                new("key", "value")
            },
            searchFields: ["abc"],
            sortOrdering: new SortOrder("Name", "asc", new List<string> { "Name" }));

        // act
        SearchResults<SearchAggregateResults, SearchFacets> result = await _sut.SearchAsync(request, CancellationToken.None);

        // verify
        _resultsMapperMock.Verify(mapper =>
            mapper.Map(It.Is<(IReadOnlyList<SearchReadModel> items,
                IReadOnlyList<AggregatedFacetResult> facets, int totalResults)>(projection =>
                    projection.items.Count == 1 &&
                    projection.items[0].Id == "100" &&
                    projection.items[0].Name == "School A" &&
                    projection.items[0].Address == "Addr, Town, County, PC" &&
                    projection.items[0].TypeName == "Type" &&
                    projection.items[0].TypeId == 12
                )), Times.Once);

    }
}
