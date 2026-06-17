using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class CompaniesHouseIdTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\r\n")]
    [InlineData("  \n  ")]
    public void Constructor_GivenNullOrWhitespace_ShouldThrowArgumentException(string? value)
    {
        // Arrange
        Func<CompaniesHouseId> construct = () => new CompaniesHouseId(value!);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_GivenValidValue_ShouldSetValue()
    {
        // Arrange
        string input = "12345678";

        // Act
        CompaniesHouseId id = new(input);

        // Assert
        Assert.Equal(input, id.Value);
    }

    [Fact]
    public void Constructor_GivenValueWithWhitespace_ShouldTrimValue()
    {
        // Arrange
        string input = " 12345678 ";

        // Act
        CompaniesHouseId id = new(input);

        // Assert
        Assert.Equal("12345678", id.Value);
    }

    [Fact]
    public void Equality_GivenSameValues_ShouldBeEqual()
    {
        // Arrange
        CompaniesHouseId first = new("12345678");
        CompaniesHouseId second = new("12345678");

        // Act & Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        CompaniesHouseId first = new("12345678");
        CompaniesHouseId second = new("87654321");

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenTrimmedEquivalentValues_ShouldBeEqual()
    {
        // Arrange
        CompaniesHouseId first = new("12345678");
        CompaniesHouseId second = new(" 12345678 ");

        // Act & Assert
        Assert.Equal(first, second);
    }


    [Fact]
    public void ToString_ShouldReturnExpectedFormat()
    {
        // Arrange
        CompaniesHouseId id = new("12345678");

        // Act
        string result = id.ToString();

        // Assert
        Assert.Equal("12345678", result);
    }
}
