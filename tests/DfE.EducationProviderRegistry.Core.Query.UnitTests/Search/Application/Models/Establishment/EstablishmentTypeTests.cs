using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment;

public sealed class EstablishmentTypeTests
{
    [Fact]
    public void Constructor_ShouldAssignValueCorrectly()
    {
        // arrange
        EstablishmentType type = new("Academy");

        // assert
        Assert.Equal("Academy", type.Value);
    }

    [Fact]
    public void FactoryMethod_ShouldReturnEquivalentInstance()
    {
        // arrange
        EstablishmentType viaCtor = new("Academy");
        EstablishmentType viaFactory = EstablishmentType.Create("Academy");

        // assert
        Assert.Equal(viaCtor, viaFactory);
        Assert.NotSame(viaCtor, viaFactory);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenValueIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new EstablishmentType(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowArgumentException_WhenValueIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            new EstablishmentType(invalid));
    }

    [Fact]
    public void FactoryMethod_ShouldThrow_WhenValueIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            EstablishmentType.Create(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void FactoryMethod_ShouldThrowArgumentException_WhenValueIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            EstablishmentType.Create(invalid));
    }
}
