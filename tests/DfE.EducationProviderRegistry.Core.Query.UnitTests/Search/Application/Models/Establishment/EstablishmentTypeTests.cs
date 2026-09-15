using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment;

public sealed class EstablishmentTypeTests
{
    [Fact]
    public void Constructor_ShouldAssignValueCorrectly()
    {
        // arrange
        SearchType type = new("Academy", 1);

        // assert
        Assert.Equal("Academy", type.Name);
    }

    [Fact]
    public void FactoryMethod_ShouldReturnEquivalentInstance()
    {
        // arrange
        SearchType viaCtor = new("Academy", 1);
        SearchType viaFactory = SearchType.Create("Academy", 1);

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
            SearchType.Create(null!, 1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void FactoryMethod_ShouldThrowArgumentException_WhenValueIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            SearchType.Create(invalid, 1));
    }
}
