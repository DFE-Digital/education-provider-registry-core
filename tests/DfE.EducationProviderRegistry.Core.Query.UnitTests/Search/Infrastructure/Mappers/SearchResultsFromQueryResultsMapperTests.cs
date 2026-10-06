using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Mappers;

public sealed class SearchResultsFromQueryResultsMapperTests
{
    [Fact]
    public void Map_Throws_WhenResultsAreNull()
    {
        // arrange
        IReadOnlyList<SearchReadModel> results = null!;
        IReadOnlyList<AggregatedFacetResult> facets = [];

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(
                () => mapper.Map((results, facets, 0)));

        // assert
        Assert.Equal("context", exception.ParamName);

        Assert.Contains(
            "Tuple does not contain search provider results.",
            exception.Message);
    }

    [Fact]
    public void Map_Throws_WhenFacetsAreNull()
    {
        // arrange
        IReadOnlyList<SearchReadModel> results = [];
        IReadOnlyList<AggregatedFacetResult> facets = null!;

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(
                () => mapper.Map((results, facets, 0)));

        // assert
        Assert.Equal("context", exception.ParamName);

        Assert.Contains(
            "Tuple does not contain facet results.",
            exception.Message);
    }

    [Fact]
    public void Map_ReturnsExpectedResults_WhenContextIsValid()
    {
        // arrange

        SearchReadModel searchReadModel =
            SearchReadModelBuilder.Create()
                .WithId("123456")
                .WithName("Test Establishment")
                .WithTypeName("Test Type")
                .WithTypeId(123)
                .WithAddress("Test Address Line, Test Addres line 2, Test City, Test County, AA1 1AA")
                .WithLocalAuthorityName("Test Local Authority")
                .WithGroup("GROUP1", "Test Group")
                .WithProviderCategory("Establishment")
                .WithAcademyCount(2)
                .Build();

        IReadOnlyList<SearchReadModel> results = [searchReadModel];

        IReadOnlyList<AggregatedFacetResult> facets =
        [
            AggregatedFacetResultBuilder.Create()
                .WithName("TestFacet")
                .WithFacetResult("value-one", "Facet label", 10)
                .Build()
        ];

        const int totalCount = 25;

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        SearchResults<SearchAggregateResults, SearchFacets> mapped =
            mapper.Map((results, facets, totalCount));

        // assert
        SearchAggregateResult mappedSearchResult =
            Assert.Single(mapped.Results!.SearchResultCollection);

        Assert.Equal("123456", mappedSearchResult.UniqueIdentifier.Value);
        Assert.Equal("Test Establishment", mappedSearchResult.Name.Value);
        Assert.Equal("Test Type", mappedSearchResult.Type?.Name);
        Assert.Equal("Test Address Line, Test Addres line 2, Test City, Test County, AA1 1AA",
            mappedSearchResult.Address?.FullAddress);
        Assert.Equal("Test Local Authority", mappedSearchResult.LocalAuthority?.Name);
        Assert.Equal("Test Group", mappedSearchResult.Group?.PartOfName);
        Assert.Equal("GROUP1", mappedSearchResult.Group?.PartOfCode);
        Assert.Equal(2, mappedSearchResult.AcademyCount);
        Assert.Equal(totalCount, mapped.TotalCount);

        SearchFacet mappedFacet =
            Assert.Single(mapped.FacetResults!.Facets);

        Assert.Equal("TestFacet", mappedFacet.Name);

        FacetResult mappedFacetResult =
            Assert.Single(mappedFacet.Results);

        Assert.Equal("value-one", mappedFacetResult.Value);
        Assert.Equal("Facet label", mappedFacetResult.Label);
        Assert.Equal(10, mappedFacetResult.Count);
    }

    [Fact]
    public void Map_UsesEmptyStrings_WhenAddressIsNull()
    {
        // arrange
        SearchReadModel searchReadModel =
            SearchReadModelBuilder.Create()
                .WithId("123456")
                .WithName("Test Establishment")
                .WithTypeName("Test Type")
                .WithTypeId(123)
                .WithAddress(null)
                .WithLocalAuthorityName("Test Local Authority")
                .WithGroup("GROUP1", "Test Group")
                .WithProviderCategory("Establishment")
                .WithAcademyCount(2)
                .Build();

        IReadOnlyList<SearchReadModel> results = [searchReadModel];

        IReadOnlyList<AggregatedFacetResult> facets = [];

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        SearchResults<SearchAggregateResults, SearchFacets> mapped =
            mapper.Map((results, facets, 1));

        // assert
        SearchAggregateResult mappedSearchResult =
            Assert.Single(mapped.Results!.SearchResultCollection);

        Assert.Equal(
            string.Empty,
            mappedSearchResult.Address?.FullAddress);
    }

    [Fact]
    public void Map_ReturnsEmptyCollections_WhenContextIsEmpty()
    {
        // arrange
        IReadOnlyList<SearchReadModel> results = [];
        IReadOnlyList<AggregatedFacetResult> facets = [];

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        SearchResults<SearchAggregateResults, SearchFacets> mapped =
            mapper.Map((results, facets, 0));

        // assert
        Assert.Empty(mapped.Results!.SearchResultCollection);
        Assert.Equal(0, mapped.TotalCount);
        Assert.Empty(mapped.FacetResults!.Facets);
    }

    [Fact]
    public void Map_MapsAllEstablishments_WhenMultipleResultsAreProvided()
    {
        // arrange
        IReadOnlyList<SearchReadModel> results =
        [
            SearchReadModelBuilder.Create()
                .WithName("Test Establishment 1")
                .WithTypeName("Test Type 1")
                .WithAddress("Test Address 1")
                .WithLocalAuthorityName("Test Local Authority 1")
                .WithGroup("GROUP1", "Test Group 1")
                .Build(),

            SearchReadModelBuilder.Create()
                .WithId("654321")
                .WithName("Test Establishment 2")
                .WithTypeName("Test Type 2")
                .WithTypeId(1321)
                .WithAddress("Test Address 2")
                .WithLocalAuthorityName("Test Local Authority 2")
                .WithGroup("GROUP2", "Test Group 2")
                .WithAcademyCount(4)
                .Build()
        ];

        IReadOnlyList<AggregatedFacetResult> facets = [];

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        SearchResults<SearchAggregateResults, SearchFacets> mapped =
            mapper.Map((results, facets, 10));

        // assert
        Assert.Collection(
            mapped.Results!.SearchResultCollection,
            first =>
            {
                Assert.Equal("123456", first.UniqueIdentifier.Value);
                Assert.Equal("Test Establishment 1", first.Name.Value);
            },
            second =>
            {
                Assert.Equal("654321", second.UniqueIdentifier.Value);
                Assert.Equal("Test Establishment 2", second.Name.Value);
            });

        Assert.Equal(10, mapped.TotalCount);
    }

    [Fact]
    public void Map_MapsAllFacetValues_WhenMultipleFacetValuesAreProvided()
    {
        // arrange
        IReadOnlyList<SearchReadModel> results = [];

        IReadOnlyList<AggregatedFacetResult> facets =
        [
            AggregatedFacetResultBuilder.Create()
                .WithName("TestFacet")
                .WithFacetResult("value-one", "Value one", 10)
                .WithFacetResult("value-two", "Value two", 20)
                .Build()
        ];

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        SearchResults<SearchAggregateResults, SearchFacets> mapped =
            mapper.Map((results, facets, 0));

        // assert
        SearchFacet mappedFacet =
            Assert.Single(mapped.FacetResults!.Facets);

        Assert.Equal("TestFacet", mappedFacet.Name);

        Assert.Collection(
            mappedFacet.Results,
            first =>
            {
                Assert.Equal("value-one", first.Value);
                Assert.Equal("Value one", first.Label);
                Assert.Equal(10, first.Count);
            },
            second =>
            {
                Assert.Equal("value-two", second.Value);
                Assert.Equal("Value two", second.Label);
                Assert.Equal(20, second.Count);
            });
    }
}
