using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared;

public sealed class UniqueReferenceNumberTests
{
    [Theory]
    [InlineData("12345")]
    [InlineData("123456")]
    [InlineData("1234567")]
    public void Constructor_ShouldAcceptValidUrns(string urn)
    {
        // Act
        UniqueReferenceNumber identifier = new(urn);

        // Assert
        Assert.Equal(urn, identifier.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("ABC123")]
    [InlineData("1234")]      // too short
    [InlineData("12345678")]  // too long
    [InlineData("12A456")]    // contains letters
    public void Constructor_ShouldThrow_WhenUrnIsInvalid(string urn)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new UniqueReferenceNumber(urn));
    }

    [Fact]
    public void ToString_ShouldReturnUrn()
    {
        // Arrange
        UniqueReferenceNumber identifier = new("123456");

        // Act
        string result = identifier.ToString();

        // Assert
        Assert.Equal("123456", result);
    }

    [Fact]
    public void Identifier_ShouldBeValueObject_AndSupportEquality()
    {
        // Arrange
        UniqueReferenceNumber a = new("123456");
        UniqueReferenceNumber b = new("123456");
        UniqueReferenceNumber c = new("654321");

        // Act & Assert
        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }
}
