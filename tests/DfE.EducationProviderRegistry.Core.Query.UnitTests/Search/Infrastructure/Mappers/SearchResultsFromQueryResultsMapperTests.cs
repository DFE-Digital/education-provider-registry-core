using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Mappers;

public sealed class SearchResultsFromQueryResultsMapperTests
{
    [Fact]
    public void Map_Throws_WhenResultsAreNull()
    {
        // arrange
        IReadOnlyList<EstablishmentReadModel> results = null!;
        IReadOnlyList<AggregatedFacetResult> facets = [];

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(
                () => mapper.Map((results, facets, 0)));

        // assert
        Assert.Equal("context", exception.ParamName);

        Assert.Contains(
            "Tuple does not contain establishment results.",
            exception.Message);
    }

    [Fact]
    public void Map_Throws_WhenFacetsAreNull()
    {
        // arrange
        IReadOnlyList<EstablishmentReadModel> results = [];
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
        EstablishmentReadModel establishment = new(
            Id: 1,
            Urn: "123456",
            Ukprn: "10012345",
            Name: "Test Establishment",
            AddressLine1: "Test Address Line",
            City: "Test City",
            County: "Test County",
            Postcode: "AA1 1AA",
            Type: "Test Type",
            Status: "Open",
            GroupName: "Test Group",
            GroupCode: "GROUP1",
            LocalAuthorityName: "Test Local Authority",
            LocalAuthorityCode: "LA1");

        IReadOnlyList<EstablishmentReadModel> results =
        [
            establishment
        ];

        IReadOnlyList<AggregatedFacetResult> facets =
        [
            new(
                "TestFacet",
                [
                    new FacetResult(
                        "facet-value",
                        "Facet label",
                        10)
                ])
        ];

        const int totalCount = 25;

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        SearchResults<EstablishmentSearchResults, SearchFacets> mapped =
            mapper.Map((results, facets, totalCount));

        // assert
        EstablishmentSearchResult mappedEstablishment =
            Assert.Single(mapped.Results!.EstablishmentCollection);

        Assert.Equal("123456", mappedEstablishment.Urn.Value);
        Assert.Equal("Test Establishment", mappedEstablishment.Name.Value);
        Assert.Equal("Test Address Line", mappedEstablishment.Address?.Street);
        Assert.Equal("Test City", mappedEstablishment.Address?.Town);
        Assert.Equal("Test County", mappedEstablishment.Address?.County);
        Assert.Equal("AA1 1AA", mappedEstablishment.Address?.Postcode);
        Assert.Equal("Test Type", mappedEstablishment.Type?.Value);

        Assert.Equal(
            "Test Group",
            mappedEstablishment.Group?.PartOfName);

        Assert.Equal(
            "GROUP1",
            mappedEstablishment.Group?.PartOfCode);

        Assert.Equal(
            "Test Local Authority",
            mappedEstablishment.LocalAuthority?.Name);

        Assert.Equal(
            "LA1",
            mappedEstablishment.LocalAuthority?.Code);

        Assert.Equal(totalCount, mapped.Results.TotalCount);

        SearchFacet mappedFacet =
            Assert.Single(mapped.FacetResults!.Facets);

        Assert.Equal("TestFacet", mappedFacet.Name);

        FacetResult mappedFacetResult =
            Assert.Single(mappedFacet.Results);

        Assert.Equal("facet-value", mappedFacetResult.Value);
        Assert.Equal("Facet label", mappedFacetResult.Label);
        Assert.Equal(10, mappedFacetResult.Count);
    }

    [Fact]
    public void Map_UsesEmptyStrings_WhenCityCountyAndPostcodeAreNull()
    {
        // arrange
        EstablishmentReadModel establishment = new(
            Id: 1,
            Urn: "123456",
            Ukprn: "10012345",
            Name: "Test Establishment",
            AddressLine1: "Test Address Line",
            City: null,
            County: null,
            Postcode: null,
            Type: "Test Type",
            Status: "Open",
            GroupName: "Test Group",
            GroupCode: "GROUP1",
            LocalAuthorityName: "Test Local Authority",
            LocalAuthorityCode: "LA1");

        IReadOnlyList<EstablishmentReadModel> results =
        [
            establishment
        ];

        IReadOnlyList<AggregatedFacetResult> facets = [];

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        SearchResults<EstablishmentSearchResults, SearchFacets> mapped =
            mapper.Map((results, facets, 1));

        // assert
        EstablishmentSearchResult mappedEstablishment =
            Assert.Single(mapped.Results!.EstablishmentCollection);

        Assert.Equal(
            string.Empty,
            mappedEstablishment.Address?.Town);

        Assert.Equal(
            string.Empty,
            mappedEstablishment.Address?.County);

        Assert.Equal(
            string.Empty,
            mappedEstablishment.Address?.Postcode);
    }

    [Fact]
    public void Map_ReturnsEmptyCollections_WhenContextIsEmpty()
    {
        // arrange
        IReadOnlyList<EstablishmentReadModel> results = [];
        IReadOnlyList<AggregatedFacetResult> facets = [];

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        SearchResults<EstablishmentSearchResults, SearchFacets> mapped =
            mapper.Map((results, facets, 0));

        // assert
        Assert.Empty(mapped.Results!.EstablishmentCollection);
        Assert.Equal(0, mapped.Results.TotalCount);
        Assert.Empty(mapped.FacetResults!.Facets);
    }

    [Fact]
    public void Map_MapsAllEstablishments_WhenMultipleResultsAreProvided()
    {
        // arrange
        IReadOnlyList<EstablishmentReadModel> results =
        [
            new(
            Id: 1,
            Urn: "123456",
            Ukprn: "10012345",
            Name: "Test Establishment One",
            AddressLine1: "Test Address One",
            City: "Test City",
            County: "Test County",
            Postcode: "AA1 1AA",
            Type: "Test Type",
            Status: "Open",
            GroupName: "Test Group",
            GroupCode: "GROUP1",
            LocalAuthorityName: "Test Local Authority",
            LocalAuthorityCode: "LA1"),

        new(
            Id: 2,
            Urn: "654321",
            Ukprn: "10054321",
            Name: "Test Establishment Two",
            AddressLine1: "Test Address Two",
            City: "Test City",
            County: "Test County",
            Postcode: "BB1 1BB",
            Type: "Test Type",
            Status: "Open",
            GroupName: "Test Group",
            GroupCode: "GROUP1",
            LocalAuthorityName: "Test Local Authority",
            LocalAuthorityCode: "LA1")
        ];

        IReadOnlyList<AggregatedFacetResult> facets = [];

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        SearchResults<EstablishmentSearchResults, SearchFacets> mapped =
            mapper.Map((results, facets, 10));

        // assert
        Assert.Collection(
            mapped.Results!.EstablishmentCollection,
            first =>
            {
                Assert.Equal("123456", first.Urn.Value);
                Assert.Equal("Test Establishment One", first.Name.Value);
            },
            second =>
            {
                Assert.Equal("654321", second.Urn.Value);
                Assert.Equal("Test Establishment Two", second.Name.Value);
            });

        Assert.Equal(10, mapped.Results.TotalCount);
    }

    [Fact]
    public void Map_MapsAllFacetValues_WhenMultipleFacetValuesAreProvided()
    {
        // arrange
        IReadOnlyList<EstablishmentReadModel> results = [];

        IReadOnlyList<AggregatedFacetResult> facets =
        [
            new(
            "TestFacet",
            [
                new FacetResult(
                    "value-one",
                    "Value one",
                    10),

                new FacetResult(
                    "value-two",
                    "Value two",
                    20)
            ])
        ];

        SearchResultsFromQueryResultsMapper mapper = new();

        // act
        SearchResults<EstablishmentSearchResults, SearchFacets> mapped =
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
