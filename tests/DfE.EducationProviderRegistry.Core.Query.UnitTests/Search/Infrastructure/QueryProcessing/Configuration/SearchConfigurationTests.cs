using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Configuration;

public sealed class SearchConfigurationTests
{
    [Fact]
    public void Constructor_SetsExpectedDefaults()
    {
        // act
        SearchConfiguration result = new();

        // assert
        Assert.Empty(result.Keys);
    }

    [Fact]
    public void Keys_WhenSet_ReturnsExpectedValues()
    {
        // arrange
        SearchIndexKeyConfiguration key = new();

        // act
        SearchConfiguration result = new()
        {
            Keys = [key]
        };

        // assert
        Assert.Single(result.Keys);
        Assert.Same(key, result.Keys[0]);
    }
}
