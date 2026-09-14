using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles.Builders;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment;

public sealed class SearchResultTests
{
    private static SearchResultBuilder Builder
    {
        get { return new SearchResultBuilder(); }
    }

    [Fact]
    public void Constructor_ShouldAssignPropertiesCorrectly()
    {
        // arrange
        SearchProviderResult result = Builder.Build();

        // assert
        Assert.Equal("12345", result.UniqueIdentifier.Value);
        Assert.Equal("Test School", result.Name.Value);
        Assert.Equal("123 Example Street", result.Address?.FullAddress);
        Assert.Equal("Academy", result.Type?.Name);
        Assert.Equal("Mock Trust", result.Group?.PartOfName);
        Assert.Equal("Test LA", result.LocalAuthority?.Name);
    }

    [Fact]
    public void FactoryMethod_ShouldReturnEquivalentInstance()
    {
        // arrange
        SearchProviderResult viaCtor =
            new(
                EstablishmentTestDouble.ValidUrn,
                EstablishmentTestDouble.ValidName,
                EstablishmentTestDouble.ValidAddress,
                EstablishmentTestDouble.ValidType,
                EstablishmentTestDouble.ValidGroup,
                EstablishmentTestDouble.ValidLocalAuthority);

        SearchProviderResult viaFactory = Builder.Build();

        // assert
        Assert.Equal(viaCtor, viaFactory);
        Assert.NotSame(viaCtor, viaFactory);
    }
}
