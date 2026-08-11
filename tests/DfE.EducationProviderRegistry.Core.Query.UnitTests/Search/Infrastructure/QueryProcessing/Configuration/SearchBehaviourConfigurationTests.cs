using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Configuration;

public sealed class SearchBehaviourConfigurationTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaults()
    {
        // act
        SearchBehaviourConfiguration result = new();

        // assert
        Assert.Equal(string.Empty, result.Name);
        Assert.Null(result.ChainingPredicate);
    }

    [Fact]
    public void Properties_WhenSet_ReturnExpectedValues()
    {
        // act
        SearchBehaviourConfiguration result = new()
        {
            Name = "exact",
            ChainingPredicate = "And"
        };

        // assert
        Assert.Equal("exact", result.Name);
        Assert.Equal("And", result.ChainingPredicate);
    }
}
