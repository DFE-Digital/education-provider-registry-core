using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class AcademyNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_GivenNullOrWhitespace_ShouldThrowArgumentException(string? value)
    {
        // Arrange
        Func<AcademyName> construct = () => new AcademyName(value!);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_GivenValidValue_ShouldSetNameValueObject()
    {
        // Arrange
        string input = "Academy";

        // Act
        AcademyName name = new(input);

        // Assert
        Assert.Equal("Academy", name.Value.Value);
    }

    [Fact]
    public void Constructor_GivenValueWithWhitespace_ShouldTrimValue()
    {
        // Arrange
        string input = " Academy ";

        // Act
        AcademyName name = new(input);

        // Assert
        Assert.Equal("Academy", name.Value.Value);
    }

    [Fact]
    public void Equality_GivenSameValues_ShouldBeEqual()
    {
        // Arrange
        AcademyName first = new("Academy");
        AcademyName second = new("Academy");

        // Act & Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        AcademyName first = new("Academy A");
        AcademyName second = new("Academy B");

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenTrimmedEquivalentValues_ShouldBeEqual()
    {
        // Arrange
        AcademyName first = new("Academy");
        AcademyName second = new(" Academy ");

        // Act & Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentUnderlyingNameObjectsWithEquivalentValues_ShouldBeEqual()
    {
        // Arrange
        AcademyName first = new("Academy");
        AcademyName second = new("academy"); // case-insensitive equality from Name

        // Act & Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void ToString_ShouldReturnUnderlyingNameValue()
    {
        // Arrange
        AcademyName name = new("Academy");

        // Act
        string result = name.ToString();

        // Assert
        Assert.Equal("Academy", result);
    }
}
