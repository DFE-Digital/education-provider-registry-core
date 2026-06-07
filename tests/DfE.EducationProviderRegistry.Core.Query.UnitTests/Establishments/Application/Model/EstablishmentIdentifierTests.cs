using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Model;

public sealed class EstablishmentIdentifierTests
{
    [Theory]
    [InlineData("12345")]
    [InlineData("123456")]
    [InlineData("1234567")]
    public void Constructor_ShouldAcceptValidUrns(string urn)
    {
        // Act
        EstablishmentIdentifier identifier = new(urn);

        // Assert
        Assert.Equal(urn, identifier.Urn);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ABC123")]
    [InlineData("1234")]      // too short
    [InlineData("12345678")]  // too long
    [InlineData("12A456")]    // contains letters
    public void Constructor_ShouldThrow_WhenUrnIsInvalid(string urn)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new EstablishmentIdentifier(urn));
    }

    [Fact]
    public void ToString_ShouldReturnUrn()
    {
        // Arrange
        EstablishmentIdentifier identifier = new("123456");

        // Act
        string result = identifier.ToString();

        // Assert
        Assert.Equal("123456", result);
    }

    [Fact]
    public void Identifier_ShouldBeValueObject_AndSupportEquality()
    {
        // Arrange
        EstablishmentIdentifier a = new("123456");
        EstablishmentIdentifier b = new("123456");
        EstablishmentIdentifier c = new("654321");

        // Act & Assert
        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
    }
}
