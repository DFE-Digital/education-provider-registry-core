using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using Tests.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Search;

public sealed class SearchResultsTests
{
    [Fact]
    public void Properties_CanBeInitializedViaObjectInitializer()
    {
        // arrange
        EstablishmentSearchResults establishmentSearchResults =
            new(
                new List<EstablishmentSearchResult>
                {
                    new(123, "Test School 1"),
                    new(456, "Test School 2")
                });

        SearchFacets facets =
            new(
                new List<SearchFacet>
                {
                    new("Region",
                    [
                        new FacetResult("North", 10)
                    ])
                });

        // act
        SearchResults<EstablishmentSearchResults, SearchFacets> result =
            new()
            {
                Results = establishmentSearchResults,
                FacetResults = facets
            };

        // Assert
        Assert.Same(establishmentSearchResults, result.Results);
        Assert.Same(facets, result.FacetResults);
    }

    [Fact]
    public void Properties_WhenUninitialized_ShouldBeNull()
    {
        // act
        SearchResults<EstablishmentSearchResults, SearchFacets> result = new();

        // Assert
        Assert.Null(result.Results);
        Assert.Null(result.FacetResults);
    }

    [Fact]
    public void CanInstantiateWithDifferentGenericTypes()
    {
        // arrange
        List<string> dummyResults = ["A", "B"];
        Dictionary<string, int> dummyFacets = new()
        {
            { "X", 1 },
            { "Y", 2 }
        };

        // act
        SearchResults<List<string>, Dictionary<string, int>> result =
            new()
            {
                Results = dummyResults,
                FacetResults = dummyFacets
            };

        // Assert
        Assert.Equal(dummyResults.Count, result.Results.Count);
        Assert.Equal(dummyFacets.Count, result.FacetResults.Count);
        Assert.True(dummyResults.CollectionsMatch(result.Results));
        Assert.True(dummyFacets.CollectionsMatch(result.FacetResults));
    }
}
