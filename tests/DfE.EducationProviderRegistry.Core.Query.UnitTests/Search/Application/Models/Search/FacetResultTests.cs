using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Search;

public sealed class FacetResultTests
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializeProperties()
    {
        // arrange
        string value = "North";
        string name = "Region";
        long? count = 42;

        // act
        FacetResult result = new(name, value, count);

        // assert
        Assert.Equal(value, result.Value);
        Assert.Equal(name, result.Key);
        Assert.Equal(count, result.Count);
    }

    [Fact]
    public void Constructor_WithNullCount_ShouldAllowNull()
    {
        // arrange
        string value = "Unspecified";
        string name = "Unspecified";
        long? count = null;

        // act
        FacetResult result = new(name, value, count);

        // assert
        Assert.Equal(value, result.Value);
        Assert.Equal(name, result.Key);
        Assert.Null(result.Count);
    }

    [Fact]
    public void Constructor_WithEmptyValue_ShouldAllowEmptyString()
    {
        // act
        FacetResult result = new(string.Empty, string.Empty, 0);

        // assert
        Assert.Equal(string.Empty, result.Value);
        Assert.Equal(string.Empty, result.Key);
        Assert.Equal(0, result.Count);
    }

    [Fact]
    public void Constructor_WithNullValue_ShouldAllowNull()
    {
        // act
        FacetResult result = new(null!, null!, 10);

        // assert
        Assert.Null(result.Value);
        Assert.Null(result.Key);
        Assert.Equal(10, result.Count);
    }
}
