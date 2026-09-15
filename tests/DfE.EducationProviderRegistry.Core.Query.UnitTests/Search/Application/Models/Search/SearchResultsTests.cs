using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Search;

public sealed class SearchResultsTests
{
    private static SearchProviderResult CreateMockEstablishment(int urn, string name)
    {
        return SearchProviderResult.Create(
            uniqueIdentifier: new ProviderIdentifier(urn.ToString("D5")),
            name: new Name(name),
            address: new SearchProviderAddress(
                "123 Example Street, " +
                "Testville, " +
                "Testshire, " +
                "TE5 7ST"),
            type: SearchProviderType.Create("Academy", 1),
            group: GroupDetail.Create("Mock Trust", "TRUST001"),
            localAuthority: SearchProviderLocalAuthority.Create("Test LA"),
            providerCategory: new SearchProviderCategory("Establishment"),
            academyCount: 10
        );
    }

    [Fact]
    public void Properties_CanBeInitializedViaObjectInitializer()
    {
        // arrange
        SearchProviderResults establishmentSearchResults =
            new(
                new List<SearchProviderResult>
                {
                    CreateMockEstablishment(123, "Test School 1"),
                    CreateMockEstablishment(456, "Test School 2"),
                });


        SearchFacets facets =
            new(
                new List<SearchFacet>
                {
                    new("Region",
                    [
                        new FacetResult("1", "North", 10)
                    ])
                });

        // act
        SearchResults<SearchProviderResults, SearchFacets> result =
            new()
            {
                Results = establishmentSearchResults,
                FacetResults = facets
            };

        // assert
        Assert.Same(establishmentSearchResults, result.Results);
        Assert.Same(facets, result.FacetResults);
    }

    [Fact]
    public void Properties_WhenUninitialized_ShouldBeNull()
    {
        // act
        SearchResults<SearchProviderResults, SearchFacets> result = new();

        // assert
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

        // assert
        Assert.Equal(dummyResults.Count, result.Results.Count);
        Assert.Equal(dummyFacets.Count, result.FacetResults.Count);
        Assert.True(dummyResults.CollectionsMatch(result.Results));
        Assert.True(dummyFacets.CollectionsMatch(result.FacetResults));
    }
}
