using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupIdTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  \n ")]
    [InlineData("\r\n")]
    public void Constructor_WhenGroupIdIsNullOrWhitespace_ThrowsArgumentException(string? input)
    {
        // Arrange
        Func<GroupId> construct = () => new GroupId(input!);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_WhenGroupIdHasValue_SetsValue()
    {
        // Arrange
        string input = "group1";

        // Act
        GroupId result = new(input);

        // Assert
        Assert.Equal("group1", result.Value);
    }

    [Fact]
    public void Constructor_WhenGroupIdHasLeadingAndTrailingWhitespace_TrimsValue()
    {
        // Arrange
        string input = "   group2   ";

        // Act
        GroupId result = new GroupId(input);

        // Assert
        Assert.Equal("group2", result.Value);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        // Arrange
        GroupId groupId = new("group3");

        // Act
        string result = groupId.ToString();

        // Assert
        Assert.Equal("group3", result);
    }

    [Fact]
    public void Equality_WhenValuesAreSame_ShouldBeEqual()
    {
        // Arrange
        GroupId left = new("group4");
        GroupId right = new("group4");

        // Act & Assert
        Assert.Equal(left, right);
        Assert.True(left.Equals(right));
        Assert.True(left == right);
    }

    [Fact]
    public void Equality_WhenValuesDiffer_ShouldNotBeEqual()
    {
        // Arrange
        GroupId left = new("group5");
        GroupId right = new("group6");

        // Act & Assert
        Assert.NotEqual(left, right);
        Assert.False(left.Equals(right));
        Assert.True(left != right);
    }

    [Fact]
    public void Equality_WhenValuesDifferOnlyByWhitespace_ShouldBeEqual()
    {
        // Arrange
        GroupId left = new("group7");
        GroupId right = new("   group7 ");

        // Act & Assert
        Assert.Equal(left, right);
        Assert.True(left.Equals(right));
    }

    [Fact]
    public void Equality_WhenComparedWithNull_ShouldBeFalse()
    {
        // Arrange
        GroupId groupId = new("group");

        // Act
        bool result = groupId.Equals(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_WhenEqual_ShouldReturnSameValue()
    {
        // Arrange
        GroupId left = new("group9");
        GroupId right = new("group9");

        // Act
        int leftHash = left.GetHashCode();
        int rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }
}
