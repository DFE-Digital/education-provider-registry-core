using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Application.Model;

public sealed class EstablishmentIdentifierTests
{
    [Fact]
    public void Constructor_Should_Throw_When_EstablishmentUrn_Is_Null()
    {
        // Arrange
        Func<EstablishmentUrnModel> construct = () => new EstablishmentUrnModel(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_Should_Set_Value_From_UniqueReferenceNumber()
    {
        // Arrange
        UniqueReferenceNumber urn = new("12345");

        // Act
        EstablishmentUrnModel result = EstablishmentUrnModel.Create(urn.Value);

        // Assert
        Assert.Equal("12345", result.Value);
    }

    [Fact]
    public void Create_Should_Throw_When_Urn_Is_Null()
    {
        // Arrange
        Func<EstablishmentUrnModel> construct = () => EstablishmentUrnModel.Create(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Create_Should_Throw_When_Urn_Is_Invalid()
    {
        // Arrange
        Func<EstablishmentUrnModel> construct = () => EstablishmentUrnModel.Create("invalid");

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Create_Should_Return_Instance_With_Expected_Value()
    {
        // Arrange
        const string urn = "12345";

        // Act
        EstablishmentUrnModel result = EstablishmentUrnModel.Create(urn);

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
        EstablishmentUrnModel fromFactory = EstablishmentUrnModel.Create(urnValue);
        EstablishmentUrnModel direct = EstablishmentUrnModel.Create(urn.Value);

        // Assert
        Assert.Equal(fromFactory, direct);
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        // Arrange
        UniqueReferenceNumber urn = new("123456");
        EstablishmentUrnModel identifier = EstablishmentUrnModel.Create(urn.Value);

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

        EstablishmentUrnModel first = EstablishmentUrnModel.Create(urn1.Value);
        EstablishmentUrnModel second = EstablishmentUrnModel.Create(urn2.Value);

        // Act & Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void Two_Instances_With_Different_Value_Should_Not_Be_Equal()
    {
        // Arrange
        UniqueReferenceNumber urn1 = new("12345");
        UniqueReferenceNumber urn2 = new("54321");

        EstablishmentUrnModel first = EstablishmentUrnModel.Create(urn1.Value);
        EstablishmentUrnModel second = EstablishmentUrnModel.Create(urn2.Value);

        // Act & Assert
        Assert.NotEqual(first, second);
    }
}
