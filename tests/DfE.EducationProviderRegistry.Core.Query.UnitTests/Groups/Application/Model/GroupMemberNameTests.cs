using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupMemberNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData(" \r\n  ")]
    public void Constructor_Should_Throw_When_FullName_Is_Null_Or_Whitespace(string? input)
    {
        // Arrange
        Func<GroupMemberName> construct =
            () => new GroupMemberName(input!);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_Should_Set_FullName()
    {
        // Arrange
        string input = "John Smith";

        // Act
        GroupMemberName result = new(input);

        // Assert
        Assert.Equal(input, result.FullName);
    }

    [Fact]
    public void Should_Be_Equal_When_FullName_Is_The_Same()
    {
        // Arrange
        GroupMemberName left = new("Jane Doe");
        GroupMemberName right = new("Jane Doe");

        // Act & Assert
        Assert.Equal(left, right);
        Assert.True(left == right);
        Assert.False(left != right);
    }

    [Fact]
    public void Should_Be_Equal_When_FullName_Differs_Only_By_Case()
    {
        // Arrange
        GroupMemberName lower = new("john smith");
        GroupMemberName upper = new("JOHN SMITH");

        // Act & Assert
        Assert.Equal(lower, upper);
        Assert.True(lower == upper);
        Assert.False(lower != upper);
    }


    [Fact]
    public void Should_Not_Be_Equal_When_FullName_Is_Different()
    {
        // Arrange
        GroupMemberName left = new("Jane Doe");
        GroupMemberName right = new("John Smith");

        // Act & Assert
        Assert.NotEqual(left, right);
        Assert.False(left == right);
        Assert.True(left != right);
    }

    [Fact]
    public void Equals_Should_Return_True_For_Same_Value_Ignoring_Case()
    {
        // Arrange
        GroupMemberName left = new("Jane Doe");
        GroupMemberName right = new("JANE DOE");

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_Should_Return_False_For_Different_Value()
    {
        // Arrange
        GroupMemberName left = new("Jane Doe");
        GroupMemberName right = new("John Smith");

        // Act
        bool result = left.Equals(right);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_Should_Be_Equal_For_Same_Value()
    {
        // Arrange
        GroupMemberName left = new("Jane Doe");
        GroupMemberName right = new("Jane Doe");

        // Act
        int leftHash = left.GetHashCode();
        int rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }

    [Fact]
    public void GetHashCode_Should_Be_Equal_When_Differing_By_Case()
    {
        // Arrange
        GroupMemberName lower = new("john smith");
        GroupMemberName upper = new("JOHN SMITH");

        // Act
        int lowerHash = lower.GetHashCode();
        int upperHash = upper.GetHashCode();

        // Assert
        Assert.Equal(lowerHash, upperHash);
    }

    [Fact]
    public void GetHashCode_Should_Be_Different_For_Different_Value()
    {
        // Arrange
        GroupMemberName left = new("Jane Doe");
        GroupMemberName right = new("John Smith");

        // Act
        int leftHash = left.GetHashCode();
        int rightHash = right.GetHashCode();

        // Assert
        Assert.NotEqual(leftHash, rightHash);
    }

    [Fact]
    public void Should_Be_Equal_When_Values_Differ_Only_By_Whitespace()
    {
        // Arrange
        GroupMemberName left = new(" John Smith ");
        GroupMemberName right = new("john smith");

        // Act & Assert
        Assert.Equal(left, right);
    }
}
