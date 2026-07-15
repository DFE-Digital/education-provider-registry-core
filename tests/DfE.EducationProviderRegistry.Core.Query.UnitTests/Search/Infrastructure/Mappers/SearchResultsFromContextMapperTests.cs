using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Mappers;

public sealed class SearchResultsFromContextMapperTests
{
    private static SearchPipelineContext BuildContextWithResults(
        EstablishmentSearchResult[] results,
        List<SearchFacet> facets)
    {
        SearchPipelineContext context = new();
        context.Set(results);
        context.Set(facets);
        return context;
    }

    [Fact]
    public void Map_Throws_WhenContextIsNull()
    {
        // arrange
        SearchResultsFromContextMapper mapper = new();

        // act // assert
        Assert.Throws<ArgumentNullException>(() => mapper.Map(null!));
    }

    [Fact]
    public void Map_Throws_WhenFacetsMissing()
    {
        // arrange
        SearchPipelineContext context = new();
        context.Set(Array.Empty<EstablishmentSearchResult>()); // results only

        SearchResultsFromContextMapper mapper = new();

        // act // assert
        InvalidOperationException ex =
            Assert.Throws<InvalidOperationException>(() => mapper.Map(context));

        Assert.Contains("List`1", ex.Message);
    }

    [Fact]
    public void Map_ReturnsExpectedResults_WhenContextIsValid()
    {
        // arrange
        EstablishmentSearchResult result1 =
            new(
                new UniqueReferenceNumber("12356"),
                new Name("School A"),
                new Address("Street A", "Town A", "County A", "AA1 1AA"),
                new EstablishmentType("Academy"),
                new GroupDetail("Group A", "Trust A", "TRUST001"),
                new LocalAuthority("LA1", "Authority A")
            );

        EstablishmentSearchResult[] resultsArray = [result1];

        SearchFacet facet1 =
            new(
                "phase",
                [new FacetResult("Primary", 1)]
            );

        List<SearchFacet> facetsList = [facet1];

        SearchPipelineContext context =
            BuildContextWithResults(resultsArray, facetsList);

        SearchResultsFromContextMapper mapper = new();

        // act
        SearchResults<EstablishmentSearchResults, SearchFacets> mapped =
            mapper.Map(context);

        // assert
        Assert.NotNull(mapped);
        Assert.NotNull(mapped.Results);
        Assert.NotNull(mapped.FacetResults);
        Assert.Single(mapped.Results.EstablishmentCollection);
        Assert.Equal("12356", mapped.Results.EstablishmentCollection.First().Urn.Value);
        Assert.Equal("School A", mapped.Results.EstablishmentCollection.First().Name.Value);
        Assert.Single(mapped.FacetResults.Facets);
        Assert.Equal("phase", mapped.FacetResults.Facets.First().Name);
        Assert.Single(mapped.FacetResults.Facets.First().Results);
        Assert.Equal("Primary", mapped.FacetResults.Facets.First().Results.First().Value);
    }
}
