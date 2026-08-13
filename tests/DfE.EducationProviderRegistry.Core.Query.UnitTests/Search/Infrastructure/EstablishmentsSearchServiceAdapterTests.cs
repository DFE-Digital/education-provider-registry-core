using System.Collections.ObjectModel;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
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

public class EstablishmentsSearchServiceAdapterTests
{
    private readonly Mock<ISearchFilterExpressionsBuilder<Establishment>> _filterBuilderMock;
    private readonly Mock<IMapper<(IReadOnlyList<EstablishmentReadModel>, IReadOnlyList<AggregatedFacetResult>),
        SearchResults<EstablishmentSearchResults, SearchFacets>>> _resultsMapperMock;
    private readonly Mock<IMapper<ReadOnlyCollection<FilterRequest>, ReadOnlyCollection<SearchFilterRequest>>> _filterMapperMock;
    private readonly EstablishmentsSearchServiceAdapter _sut;
    private readonly EducationProviderRegistryDbContext _db;

    public EstablishmentsSearchServiceAdapterTests()
    {
        DbContextOptions<EducationProviderRegistryDbContext> options =
            new DbContextOptionsBuilder<EducationProviderRegistryDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        _db = new EducationProviderRegistryDbContext(options);

        Mock<ISearchQueryProcessor<Establishment>>  searchProcessorMock = SearchQueryProcessorTestDouble.Mock();
        Mock<IFacetAggregator> facetAggregatorMock = FacetAggregatorTestDouble.Mock();

        _filterBuilderMock = SearchFilterExpressionsBuilderTestDouble.Mock();
        _resultsMapperMock = ResultsMapperTestDouble.Mock();
        _filterMapperMock = FilterMapperTestDouble.Mock();

        _sut = new EstablishmentsSearchServiceAdapter(
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
        _db.Establishment.Add(new Establishment
        {
            EstablishmentId = 1,
            Urn = "100",
            Uid = "UID",
            Name = "School A",
            Site =
            [
                new() { AddressLine1 = "Addr", Town = "Town", County = "County", Postcode = "PC" }
            ],
            EstablishmentType =
                new Data.DatabaseModels.Models.EstablishmentType
            {
                Name = "Type",
                Code = "T"
            },
            EstablishmentStatus =
                new EstablishmentStatus
            {
                Name = "Status",
                Code = "ST"
            },
            EstablishmentGroupMembership =
            [
                new EstablishmentGroupMembership
                {
                    Group =
                        new GroupRecord
                        {
                            Name = "Group",
                            Code = "GC"
                        }
                }
            ],
            EstablishmentAuthority =
            [
                new EstablishmentAuthority
                {
                    AuthorityName = "Auth",
                    AuthorityCode = "AC"
                }
            ]
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
        SearchResults<EstablishmentSearchResults, SearchFacets> result = await _sut.SearchAsync(request, CancellationToken.None);

        // verify
        _resultsMapperMock.Verify(mapper =>
            mapper.Map(It.Is<(IReadOnlyList<EstablishmentReadModel> items,
                IReadOnlyList<AggregatedFacetResult> facets)>(projection =>
                    projection.items.Count == 1 &&
                    projection.items[0].Urn == "100" &&
                    projection.items[0].Name == "School A" &&
                    projection.items[0].AddressLine1 == "Addr" &&
                    projection.items[0].City == "Town" &&
                    projection.items[0].County == "County" &&
                    projection.items[0].Postcode == "PC" &&
                    projection.items[0].Type == "Type" &&
                    projection.items[0].Status == "Status" &&
                    projection.items[0].GroupName == "Group" &&
                    projection.items[0].GroupCode == "GC" &&
                    projection.items[0].LocalAuthorityName == "Auth" &&
                    projection.items[0].LocalAuthorityCode == "AC"
                )), Times.Once);
    }
}
