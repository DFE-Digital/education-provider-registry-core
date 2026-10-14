using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Search;

public sealed class SearchTermTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void Constructor_WhenKeyIsNullOrWhitespace_ThrowsArgumentException(string? key)
    {
        // Arrange
        Func<SearchTerm> construct = () => new SearchTerm(key!, "value");

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_WhenKeyContainsLeadingAndTrailingWhitespace_TrimsKey()
    {
        // Arrange
        string key = "  name  ";

        // Act
        SearchTerm result = new(key, "value");

        // Assert
        Assert.Equal("name", result.Key);
    }

    [Fact]
    public void Constructor_WhenValueIsNull_SetsValueToEmptyString()
    {
        // Arrange & Act
        SearchTerm result = new("name", null);

        // Assert
        Assert.Equal(string.Empty, result.Value);
    }

    [Fact]
    public void Constructor_WhenValueContainsLeadingAndTrailingWhitespace_TrimsValue()
    {
        // Arrange
        string value = "  search value  ";

        // Act
        SearchTerm result = new("name", value);

        // Assert
        Assert.Equal("search value", result.Value);
    }

    [Fact]
    public void Constructor_WhenValueIsWhitespaceOnly_SetsValueToEmptyString()
    {
        // Arrange
        string value = "   ";

        // Act
        SearchTerm result = new("name", value);

        // Assert
        Assert.Equal(string.Empty, result.Value);
    }

    [Fact]
    public void Constructor_WhenValidValuesProvided_SetsProperties()
    {
        // Arrange
        string key = "name";
        string value = "john";

        // Act
        SearchTerm result = new(key, value);

        // Assert
        Assert.Equal(key, result.Key);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Constructor_WhenKeyAndValueRequireTrimming_TrimsBothProperties()
    {
        // Arrange
        string key = "  name  ";
        string value = "  john  ";

        // Act
        SearchTerm result = new(key, value);

        // Assert
        Assert.Equal("name", result.Key);
        Assert.Equal("john", result.Value);
    }
}
