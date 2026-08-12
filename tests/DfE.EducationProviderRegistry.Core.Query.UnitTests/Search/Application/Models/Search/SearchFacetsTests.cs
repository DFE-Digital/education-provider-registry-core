using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Search;

public sealed class SearchFacetsTests
{
    [Fact]
    public void DefaultConstructor_ShouldInitializeEmptyFacetCollection()
    {
        // act
        SearchFacets facets = new();

        // Assert
        Assert.NotNull(facets.Facets);
        Assert.Empty(facets.Facets);
    }

    [Fact]
    public void Constructor_WithValidFacetList_ShouldInitializeFacets()
    {
        // arrange
        List<SearchFacet> facetList =
        [
            new SearchFacet("Region", [new FacetResult("1", "North", 10)]),
            new SearchFacet("Provider", [new FacetResult("1", "College A", 5)])
        ];

        // act
        SearchFacets facets = new(facetList);

        // assert
        Assert.Equal(facetList.Count, facets.Facets.Count);

        Assert.True(facets.Facets.CollectionsMatch(
            facetList,
            (expected, actual) =>
                expected.Name == actual.Name &&
                actual.Results.CollectionsMatch(
                    expected.Results,
                    (expected, actual) =>
                        expected.Value == actual.Value &&
                        expected.Count == actual.Count)));
    }

    [Fact]
    public void Facets_ShouldBeReadOnly()
    {
        // arrange
        List<SearchFacet> facetList =
        [
            new SearchFacet("Subject", [new FacetResult("1", "Math", 3)])
        ];

        SearchFacets facets = new(facetList);

        // act
        IReadOnlyCollection<SearchFacet> readOnlyFacets = facets.Facets;

        // assert
        Assert.Throws<InvalidCastException>(() =>
        {
            List<SearchFacet> list = (List<SearchFacet>)readOnlyFacets;
            list.Add(new SearchFacet("Extra", []));
        });
    }

    [Fact]
    public void Constructor_WithEmptyEnumerable_ShouldInitializeEmptyCollection()
    {
        // act
        SearchFacets facets = new(new List<SearchFacet>());

        // assert
        Assert.Empty(facets.Facets);
    }
}
