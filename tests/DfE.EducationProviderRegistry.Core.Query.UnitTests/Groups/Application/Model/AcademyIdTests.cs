using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class AcademyIdTests
{
    [Fact]
    public void Constructor_GivenNullUrn_ShouldThrowArgumentNullException()
    {
        // Arrange
        UniqueReferenceNumber urn = null!;

        Func<AcademyId> construct = () => new AcademyId(urn);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_GivenValidUrn_ShouldSetValue()
    {
        // Arrange
        UniqueReferenceNumber urn = UniqueReferenceNumberTestDoubles.Create("123456");

        // Act
        AcademyId identifier = new(urn);

        // Assert
        Assert.Equal("123456", identifier.Value);
    }

    [Fact]
    public void Equality_GivenSameUrnValues_ShouldBeEqual()
    {
        // Arrange
        UniqueReferenceNumber urn1 = UniqueReferenceNumberTestDoubles.Create("123456");
        UniqueReferenceNumber urn2 = UniqueReferenceNumberTestDoubles.Create("123456");

        AcademyId first = new(urn1);
        AcademyId second = new(urn2);

        // Act & Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentUrnValues_ShouldNotBeEqual()
    {
        // Arrange
        AcademyId first = new(
            UniqueReferenceNumberTestDoubles.Create("123456"));

        AcademyId second = new(
            UniqueReferenceNumberTestDoubles.Create("654321"));

        // Act & Assert
        Assert.NotEqual(first, second);
    }
}
