using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.Test.Shared.TestDoubles.Groups;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupCharacteristicsTests
{
    [Fact]
    public void Constructor_WhenNameIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<GroupCharacteristics> construct =
            () => new(
                    null!,
                    AddressTestDoubles.Generate(),
                    GroupTypeTestDoubles.Stub(),
                    GroupStatusTestDoubles.Create());

        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenAddressIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<GroupCharacteristics> construct =
            () => new(
                    NameTestDoubles.Create(),
                    null!,
                    GroupTypeTestDoubles.Stub(),
                    GroupStatusTestDoubles.Create());

        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenTypeIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Func<GroupCharacteristics> construct =
            () => new(
                    NameTestDoubles.Create(),
                    AddressTestDoubles.Generate(),
                    null!,
                    GroupStatusTestDoubles.Create());

        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void Constructor_WhenAllArgumentsProvided_AssignsProperties()
    {
        // Arrange
        Name name = NameTestDoubles.Create();
        SiteAddressModel address = AddressTestDoubles.Generate();
        GroupType type = GroupTypeTestDoubles.Stub();
        GroupStatus status = GroupStatusTestDoubles.Create();

        // Act
        GroupCharacteristics result = new(name, address, type, status);

        // Assert
        Assert.Same(name, result.Name);
        Assert.Same(address, result.Address);
        Assert.Same(type, result.Type);
        Assert.Same(status, result.Status);
    }

    [Fact]
    public void Equals_WhenAllPropertiesMatch_ReturnsTrue()
    {
        // Arrange

        GroupCharacteristics left = new(
            NameTestDoubles.Create("my-name"),
            AddressTestDoubles.Stub(),
            GroupTypeTestDoubles.Create("MAT"),
            GroupStatusTestDoubles.Create("Opened"));

        GroupCharacteristics right = new(
            NameTestDoubles.Create("my-name"),
            AddressTestDoubles.Stub(),
            GroupTypeTestDoubles.Create("MAT"),
            GroupStatusTestDoubles.Create("Opened"));

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_WhenNameDiffers_ReturnsFalse()
    {
        // Arrange
        GroupCharacteristics left = new(
            NameTestDoubles.Create("my-name-1"),
            AddressTestDoubles.Stub(),
            GroupTypeTestDoubles.Stub(),
            GroupStatusTestDoubles.Create());

        GroupCharacteristics right = new(
            NameTestDoubles.Create("my-name-2"),
            AddressTestDoubles.Stub(),
            GroupTypeTestDoubles.Stub(),
            GroupStatusTestDoubles.Create());

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WhenAddressDiffers_ReturnsFalse()
    {
        // Arrange
        GroupCharacteristics left = new(
            NameTestDoubles.Create(),
            AddressTestDoubles.Create(AddressLine1: "Address A"),
            GroupTypeTestDoubles.Stub(),
            GroupStatusTestDoubles.Create());

        GroupCharacteristics right = new(
            NameTestDoubles.Create(),
            AddressTestDoubles.Create(AddressLine1: "Address B"),
            GroupTypeTestDoubles.Stub(),
            GroupStatusTestDoubles.Create());

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WhenTypeDiffers_ReturnsFalse()
    {
        // Arrange
        GroupCharacteristics left = new(
            NameTestDoubles.Create(),
            AddressTestDoubles.Generate(),
            GroupTypeTestDoubles.Create("Type A"),
            GroupStatusTestDoubles.Create());

        GroupCharacteristics right = new(
            NameTestDoubles.Create(),
            AddressTestDoubles.Generate(),
            GroupTypeTestDoubles.Create("Type B"),
            GroupStatusTestDoubles.Create());

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_WhenStatusDiffers_ReturnsFalse()
    {
        // Arrange
        GroupCharacteristics left = new(
            NameTestDoubles.Create(),
            AddressTestDoubles.Generate(),
            GroupTypeTestDoubles.Stub(),
            GroupStatusTestDoubles.Create("Opened"));

        GroupCharacteristics right = new(
            NameTestDoubles.Create(),
            AddressTestDoubles.Generate(),
            GroupTypeTestDoubles.Stub(),
            GroupStatusTestDoubles.Create("Closed"));

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }
}
