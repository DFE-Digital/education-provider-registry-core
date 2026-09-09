using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment;

public sealed class LocalAuthorityTests
{
    [Fact]
    public void Constructor_ShouldAssignPropertiesCorrectly()
    {
        // arrange
        SearchProviderLocalAuthority authority = new("Test LA", "LA001");

        // assert
        Assert.Equal("Test LA", authority.Name);
        Assert.Equal("LA001", authority.Code);
    }

    [Fact]
    public void FactoryMethod_ShouldReturnEquivalentInstance()
    {
        // arrange
        SearchProviderLocalAuthority viaCtor = new("Test LA", "LA001");
        SearchProviderLocalAuthority viaFactory = SearchProviderLocalAuthority.Create("Test LA", "LA001");

        // assert
        Assert.Equal(viaCtor, viaFactory);
        Assert.NotSame(viaCtor, viaFactory);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenNameIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new LocalAuthority(null!, "LA001"));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenCodeIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new LocalAuthority("Test LA", null!));
    }

    [Fact]
    public void FactoryMethod_ShouldThrow_WhenNameIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            SearchProviderLocalAuthority.Create(null!, "LA001"));
    }

    [Fact]
    public void FactoryMethod_ShouldThrow_WhenCodeIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            SearchProviderLocalAuthority.Create("Test LA", null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowArgumentException_WhenNameIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            new LocalAuthority(invalid, "LA001"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_ShouldThrowArgumentException_WhenCodeIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            new LocalAuthority("Test LA", invalid));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void FactoryMethod_ShouldThrowArgumentException_WhenNameIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            SearchProviderLocalAuthority.Create(invalid, "LA001"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void FactoryMethod_ShouldThrowArgumentException_WhenCodeIsEmptyOrWhitespace(string invalid)
    {
        // arrange/assert
        Assert.Throws<ArgumentException>(() =>
            SearchProviderLocalAuthority.Create("Test LA", invalid));
    }
}
