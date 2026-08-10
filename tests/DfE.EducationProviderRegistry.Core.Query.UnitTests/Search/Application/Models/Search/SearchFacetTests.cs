using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Search;

public sealed class SearchFacetTests
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializeProperties()
    {
        // arrange
        string facetName = "Region";

        List<FacetResult> facetResults =
        [
            new FacetResult("1", "North", 10),
            new FacetResult("2", "South", 5)
        ];

        // act
        SearchFacet facet = new(facetName, facetResults);

        // assert
        Assert.Equal(facetName, facet.Name);
        Assert.Equal(facetResults.Count, facet.Results.Count);

        Assert.True(facet.Results.CollectionsMatch(
            facetResults,
            (expected, actual) =>
                expected.Value == actual.Value &&
                expected.Count == actual.Count));
    }

    [Fact]
    public void Constructor_WithEmptyResults_ShouldAllowEmptyList()
    {
        // arrange
        SearchFacet facet = new("Provider", []);

        // assert
        Assert.Equal("Provider", facet.Name);
        Assert.Empty(facet.Results);
    }

    [Fact]
    public void Constructor_WithNullName_ShouldAllowNull()
    {
        // arrange
        List<FacetResult> results =
        [
            new FacetResult("1", "Unspecified", 0)
        ];

        // act
        SearchFacet facet = new(null!, results);

        // assert
        Assert.Null(facet.Name);
        Assert.Equal(results.Count, facet.Results.Count);

        Assert.True(facet.Results.CollectionsMatch(
            results,
            (expected, actual) =>
                expected.Value == actual.Value &&
                expected.Count == actual.Count));
    }

    [Fact]
    public void Constructor_WithNullResults_ShouldAllowNull()
    {
        // act
        SearchFacet facet = new("Gender", null!);

        // assert
        Assert.Equal("Gender", facet.Name);
        Assert.Null(facet.Results);
    }
}
