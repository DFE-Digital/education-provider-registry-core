using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment;

public sealed class EstablishmentTypeTests
{
    [Fact]
    public void Constructor_ShouldAssignValueCorrectly()
    {
        // arrange
        SearchProviderType type = new("Academy");

        // assert
        Assert.Equal("Academy", type.Value);
    }

    [Fact]
    public void FactoryMethod_ShouldReturnEquivalentInstance()
    {
        // arrange
        SearchProviderType viaCtor = new("Academy");
        SearchProviderType viaFactory = SearchProviderType.Create("Academy");

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
            SearchProviderType.Create(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void FactoryMethod_ShouldThrowArgumentException_WhenValueIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            SearchProviderType.Create(invalid));
    }
}
