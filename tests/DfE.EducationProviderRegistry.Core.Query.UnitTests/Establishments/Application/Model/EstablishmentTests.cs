using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Model;

public sealed class EstablishmentTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenIdentifierIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Establishment(null!));
    }

    [Fact]
    public void Constructor_ShouldSetIdentifier()
    {
        // Arrange
        UniqueReferenceNumber urn = new("123456");

        EstablishmentIdentifier identifier = new(urn);

        // Act
        Establishment establishment = new(identifier);

        // Assert
        Assert.Equal(identifier, establishment.Identifier);
    }

    [Fact]
    public void Create_ShouldReturnNewInstanceWithIdentifier()
    {
        // Arrange
        UniqueReferenceNumber urn = new("654321");

        EstablishmentIdentifier identifier = new(urn);

        // Act
        Establishment establishment = Establishment.Create(identifier);

        // Assert
        Assert.Equal(identifier, establishment.Identifier);
    }
}
