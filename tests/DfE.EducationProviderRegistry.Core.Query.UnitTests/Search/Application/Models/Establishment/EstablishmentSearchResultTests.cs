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
        Assert.Equal("123 Example Street", result.Address?.AddressLine1);
        Assert.Equal("Academy", result.Type?.Value);
        Assert.Equal("Mock Trust", result.Group?.PartOfName);
        Assert.Equal("Test LA", result.LocalAuthority?.Name);
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
}
