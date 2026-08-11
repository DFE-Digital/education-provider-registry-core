using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Configuration;

public sealed class SearchIndexKeyConfigurationTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaults()
    {
        // act
        SearchIndexKeyConfiguration result = new();

        // assert
        Assert.Equal(string.Empty, result.SearchTermKey);
        Assert.Null(result.ChainingPredicate);
        Assert.Empty(result.IndexedFields);
    }

    [Fact]
    public void Properties_WhenSet_ReturnExpectedValues()
    {
        // arrange
        IndexedFieldConfiguration indexedField = new();

        // act
        SearchIndexKeyConfiguration result = new()
        {
            SearchTermKey = "Name",
            ChainingPredicate = "And",
            IndexedFields = [indexedField]
        };

        // assert
        Assert.Equal("Name", result.SearchTermKey);
        Assert.Equal("And", result.ChainingPredicate);
        Assert.Single(result.IndexedFields);
        Assert.Same(indexedField, result.IndexedFields[0]);
    }
}
