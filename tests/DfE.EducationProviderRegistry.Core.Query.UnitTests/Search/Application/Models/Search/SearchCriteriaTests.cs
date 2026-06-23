using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Search;

public sealed class SearchCriteriaTests
{
    [Fact]
    public void Constructor_ShouldInitializeEmptyCollections()
    {
        // act
        SearchCriteria criteria = new();

        // assert
        Assert.NotNull(criteria.SearchFields);
        Assert.Empty(criteria.SearchFields);
        Assert.NotNull(criteria.Facets);
        Assert.Empty(criteria.Facets);
    }

    [Fact]
    public void Properties_CanBeAssignedAndRetrieved()
    {
        // arrange
        List<string> fields = ["Name", "Subject"];
        List<string> facets = ["Region", "Provider"];

        SearchCriteria criteria =
            new()
            {
                SearchFields = fields,
                Facets = facets
            };

        // assert
        Assert.Equal(fields.Count, criteria.SearchFields.Count);
        Assert.Equal(facets.Count, criteria.Facets.Count);
        Assert.True(fields.CollectionsMatch(criteria.SearchFields));
        Assert.True(facets.CollectionsMatch(criteria.Facets));
    }

    [Fact]
    public void Properties_CanBeMutatedAfterInitialization()
    {
        // arrange
        SearchCriteria criteria = new();

        // act
        criteria.SearchFields.Add("Level");
        criteria.Facets.Add("Gender");

        // assert
        Assert.Single(criteria.SearchFields);
        Assert.Equal("Level", criteria.SearchFields[0]);

        Assert.Single(criteria.Facets);
        Assert.Equal("Gender", criteria.Facets[0]);
    }
}
