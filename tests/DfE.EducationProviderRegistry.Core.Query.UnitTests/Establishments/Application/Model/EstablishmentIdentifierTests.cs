using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Model;

public sealed class EstablishmentIdentifierTests
{
    [Fact]
    public void Constructor_Should_Throw_When_EstablishmentIdentifier_Is_Null()
    {
        // Arrange
        Func<EstablishmentIdentifier> construct = () => new EstablishmentIdentifier(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Should_Set_Value_From_UniqueReferenceNumber()
    {
        // Arrange
        UniqueReferenceNumber urn = new("12345");

        // Act
        EstablishmentIdentifier result = new(urn);

        // Assert
        Assert.Equal("12345", result.Value);
    }

    [Fact]
    public void Create_Should_Throw_When_Urn_Is_Null()
    {
        // Arrange
        Func<EstablishmentIdentifier> construct = () => EstablishmentIdentifier.Create(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Create_Should_Throw_When_Urn_Is_Invalid()
    {
        // Arrange
        Func<EstablishmentIdentifier> construct = () => EstablishmentIdentifier.Create("invalid");

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Create_Should_Return_Instance_With_Expected_Value()
    {
        // Arrange
        const string urn = "12345";

        // Act
        EstablishmentIdentifier result = EstablishmentIdentifier.Create(urn);

        // Assert
        Assert.Equal(urn, result.Value);
    }

    [Fact]
    public void Create_Should_Return_Equivalent_Instance_To_Direct_Construction()
    {
        // Arrange
        const string urnValue = "123456";
        UniqueReferenceNumber urn = new(urnValue);

        // Act
        EstablishmentIdentifier fromFactory = EstablishmentIdentifier.Create(urnValue);
        EstablishmentIdentifier direct = new(urn);

        // Assert
        Assert.Equal(fromFactory, direct);
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        // Arrange
        UniqueReferenceNumber urn = new("123456");
        EstablishmentIdentifier identifier = new(urn);

        // Act
        string result = identifier.ToString();

        // Assert
        Assert.Equal("123456", result);
    }

    [Fact]
    public void Two_Instances_With_Same_Value_Should_Be_Equal()
    {
        // Arrange
        UniqueReferenceNumber urn1 = new("1234567");
        UniqueReferenceNumber urn2 = new("1234567");

        EstablishmentIdentifier first = new(urn1);
        EstablishmentIdentifier second = new(urn2);

        // Act & Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void Two_Instances_With_Different_Value_Should_Not_Be_Equal()
    {
        // Arrange
        UniqueReferenceNumber urn1 = new("12345");
        UniqueReferenceNumber urn2 = new("54321");

        EstablishmentIdentifier first = new(urn1);
        EstablishmentIdentifier second = new(urn2);

        // Act & Assert
        Assert.NotEqual(first, second);
    }
}
