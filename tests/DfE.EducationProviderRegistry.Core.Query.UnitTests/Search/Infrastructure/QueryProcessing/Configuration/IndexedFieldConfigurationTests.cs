using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Configuration;

public sealed class IndexedFieldConfigurationTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaults()
    {
        // act
        IndexedFieldConfiguration result = new();

        // assert
        Assert.Equal(string.Empty, result.FieldName);
        Assert.Null(result.DefaultBehaviourChainingPredicate);
        Assert.Empty(result.SearchBehaviours);
    }

    [Fact]
    public void Properties_WhenSet_ReturnExpectedValues()
    {
        // arrange
        SearchBehaviourConfiguration behaviour = new();

        // act
        IndexedFieldConfiguration result = new()
        {
            FieldName = "Name",
            DefaultBehaviourChainingPredicate = "And",
            SearchBehaviours = [behaviour]
        };

        // assert
        Assert.Equal("Name", result.FieldName);
        Assert.Equal("And", result.DefaultBehaviourChainingPredicate);
        Assert.Single(result.SearchBehaviours);
        Assert.Same(behaviour, result.SearchBehaviours[0]);
    }
}
