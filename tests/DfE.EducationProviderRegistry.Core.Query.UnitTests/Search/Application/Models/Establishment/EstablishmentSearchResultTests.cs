using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles.Builders;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment;

public sealed class EstablishmentSearchResultTests
{
    private static EstablishmentSearchResultBuilder Builder
    {
        get { return new EstablishmentSearchResultBuilder(); }
    }

    [Fact]
    public void Constructor_ShouldAssignPropertiesCorrectly()
    {
        // arrange
        EstablishmentSearchResult result = Builder.Build();

        // assert
        Assert.Equal("12345", result.Urn.Value);
        Assert.Equal("Test School", result.Name.Value);
        Assert.Equal("123 Example Street", result.Address.Street);
        Assert.Equal("Academy", result.Type.Value);
        Assert.Equal("Mock Trust", result.Group.PartOfName);
        Assert.Equal("Test LA", result.LocalAuthority.Name);
    }

    [Fact]
    public void FactoryMethod_ShouldReturnEquivalentInstance()
    {
        // arrange
        EstablishmentSearchResult viaCtor =
            new(
                EstablishmentTestDouble.ValidUrn,
                EstablishmentTestDouble.ValidName,
                EstablishmentTestDouble.ValidAddress,
                EstablishmentTestDouble.ValidType,
                EstablishmentTestDouble.ValidGroup,
                EstablishmentTestDouble.ValidLocalAuthority);

        EstablishmentSearchResult viaFactory = Builder.Build();

        // assert
        Assert.Equal(viaCtor, viaFactory);
        Assert.NotSame(viaCtor, viaFactory);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenUrnIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new EstablishmentSearchResult(
                null!,
                EstablishmentTestDouble.ValidName,
                EstablishmentTestDouble.ValidAddress,
                EstablishmentTestDouble.ValidType,
                EstablishmentTestDouble.ValidGroup,
                EstablishmentTestDouble.ValidLocalAuthority));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenNameIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new EstablishmentSearchResult(
                EstablishmentTestDouble.ValidUrn,
                null!,
                EstablishmentTestDouble.ValidAddress,
                EstablishmentTestDouble.ValidType,
                EstablishmentTestDouble.ValidGroup,
                EstablishmentTestDouble.ValidLocalAuthority));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenAddressIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new EstablishmentSearchResult(
                EstablishmentTestDouble.ValidUrn,
                EstablishmentTestDouble.ValidName,
                null!,
                EstablishmentTestDouble.ValidType,
                EstablishmentTestDouble.ValidGroup,
                EstablishmentTestDouble.ValidLocalAuthority));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenTypeIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new EstablishmentSearchResult(
                EstablishmentTestDouble.ValidUrn,
                EstablishmentTestDouble.ValidName,
                EstablishmentTestDouble.ValidAddress,
                null!,
                EstablishmentTestDouble.ValidGroup,
                EstablishmentTestDouble.ValidLocalAuthority));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenGroupIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new EstablishmentSearchResult(
                EstablishmentTestDouble.ValidUrn,
                EstablishmentTestDouble.ValidName,
                EstablishmentTestDouble.ValidAddress,
                EstablishmentTestDouble.ValidType,
                null!,
                EstablishmentTestDouble.ValidLocalAuthority));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenLocalAuthorityIsNull()
    {
        // arrange/assert
        Assert.Throws<ArgumentNullException>(() =>
            new EstablishmentSearchResult(
                EstablishmentTestDouble.ValidUrn,
                EstablishmentTestDouble.ValidName,
                EstablishmentTestDouble.ValidAddress,
                EstablishmentTestDouble.ValidType,
                EstablishmentTestDouble.ValidGroup,
                null!));
    }
}
