using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupIdentifierTests
{
    [Fact]
    public void Constructor_Should_Throw_ArgumentNullException_When_Value_Is_Null()
    {
        // Arrange
        Func<GroupIdentifier> construct = () => new(null!);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  \n ")]
    [InlineData("  \r\n ")]
    public void Constructor_Should_Throw_When_GroupId_Is_Whitespace(string groupId)
    {
        // Arrange
        Func<GroupIdentifier> construct = () => new GroupIdentifier(groupId);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_Should_Set_Value()
    {
        // Arrange
        string groupId = "ABC123";

        // Act
        GroupIdentifier result = new(groupId);

        // Assert
        Assert.Equal("ABC123", result.Value);
    }

    [Fact]
    public void Constructor_Should_Trim_Value()
    {
        // Arrange
        string groupId = "  ABC123  ";

        // Act
        GroupIdentifier result = new(groupId);

        // Assert
        Assert.Equal("ABC123", result.Value);
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        // Arrange
        GroupIdentifier identifier = new("ABC123");

        // Act
        string result = identifier.ToString();

        // Assert
        Assert.Equal("ABC123", result);
    }

    [Fact]
    public void Two_Instances_With_Same_Value_Should_Be_Equal()
    {
        // Arrange
        GroupIdentifier first = new("ABC123");
        GroupIdentifier second = new("ABC123");

        // Act & Assert
        Assert.Equal(first, second);
    }

    [Fact]
    public void Two_Instances_With_Different_Value_Should_Not_Be_Equal()
    {
        // Arrange
        GroupIdentifier first = new("ABC123");
        GroupIdentifier second = new("XYZ789");

        // Act & Assert
        Assert.NotEqual(first, second);
    }
}
