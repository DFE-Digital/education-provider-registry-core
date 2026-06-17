using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;


public sealed class AcademyTests
{
    [Fact]
    public void Constructor_GivenNullId_ShouldThrowArgumentNullException()
    {
        // Arrange
        AcademyId id = null!;
        AcademyName name = AcademyNameTestDoubles.Create();

        Func<Academy> construct = () => new Academy(id, name);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_GivenNullName_ShouldThrowArgumentNullException()
    {
        // Arrange
        AcademyId id = AcademyIdTestDoubles.Create();
        AcademyName name = null!;

        Func<Academy> construct = () => new Academy(id, name);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_GivenValidArguments_ShouldSetProperties()
    {
        // Arrange
        AcademyId id = AcademyIdTestDoubles.Create();
        AcademyName name = AcademyNameTestDoubles.Create();

        // Act
        Academy academy = new(id, name);

        // Assert
        Assert.Equal(id, academy.Id);
        Assert.Equal(name, academy.Name);
    }

    [Fact]
    public void Equality_GivenSameValues_ShouldBeEqual()
    {
        // Arrange
        AcademyId id = AcademyIdTestDoubles.Create();
        AcademyName name = AcademyNameTestDoubles.Create();

        Academy first = new(id, name);
        Academy second = new(id, name);

        // Act & Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentId_ShouldNotBeEqual()
    {
        // Arrange
        Academy first = new(
            AcademyIdTestDoubles.Create("12345"),
            AcademyNameTestDoubles.Create("Academy"));

        Academy second = new(
            AcademyIdTestDoubles.Create("54321"),
            AcademyNameTestDoubles.Create("Academy"));

        // Act & Assert
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Equality_GivenDifferentName_ShouldNotBeEqual()
    {
        // Arrange
        Academy first = new(
            AcademyIdTestDoubles.Create("12345"),
            AcademyNameTestDoubles.Create("Academy A"));

        Academy second = new(
            AcademyIdTestDoubles.Create("12345"),
            AcademyNameTestDoubles.Create("Academy B"));

        // Act & Assert
        Assert.NotEqual(first, second);
    }
}
