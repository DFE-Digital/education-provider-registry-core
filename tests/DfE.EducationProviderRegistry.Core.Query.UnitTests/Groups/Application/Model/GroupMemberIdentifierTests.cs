using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupMemberIdentifierTests
{

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData(" \r\n  ")]
    public void Constructor_Should_Throw_When_Value_Is_Null_Or_Whitespace(string? input)
    {
        // Arrange
        Func<GroupMemberIdentifier> construct =
            () => new GroupMemberIdentifier(input!);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_Should_Set_Value()
    {
        // Arrange
        string input = "governor-123";

        // Act
        GroupMemberIdentifier result = new(input);

        // Assert
        Assert.Equal(input, result.Value);
    }

    [Fact]
    public void ToString_Should_Return_Value()
    {
        // Arrange
        string input = "governor-456";
        GroupMemberIdentifier sut = new(input);

        // Act
        string result = sut.ToString();

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void Create_Should_Return_Instance_With_Value()
    {
        // Arrange
        string input = "governor-789";

        // Act
        GroupMemberIdentifier result = GroupMemberIdentifier.Create(input);

        // Assert
        Assert.Equal(input, result.Value);
    }

    [Fact]
    public void Create_Should_Throw_When_Input_Is_Invalid()
    {
        // Arrange
        Func<GroupMemberIdentifier> act =
            () => GroupMemberIdentifier.Create("");

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(act);
    }


    [Fact]
    public void Should_Be_Equal_When_Values_Are_The_Same()
    {
        // Arrange
        GroupMemberIdentifier first = new("id-123");
        GroupMemberIdentifier second = new("id-123");

        // Act & Assert
        Assert.Equal(first, second);
        Assert.True(first == second);
    }

    [Fact]
    public void Should_Not_Be_Equal_When_Values_Are_Different()
    {
        // Arrange
        GroupMemberIdentifier first = new("id-123");
        GroupMemberIdentifier second = new("id-456");

        // Act & Assert
        Assert.NotEqual(first, second);
        Assert.True(first != second);
    }

    [Fact]
    public void Equals_Should_Return_True_For_Same_Value()
    {
        // Arrange
        GroupMemberIdentifier first = new("id-123");
        GroupMemberIdentifier second = new("id-123");

        // Act
        bool result = first.Equals(second);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_Should_Return_False_For_Different_Value()
    {
        // Arrange
        GroupMemberIdentifier first = new("id-123");
        GroupMemberIdentifier second = new("id-456");

        // Act
        bool result = first.Equals(second);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetHashCode_Should_Be_Equal_For_Same_Value()
    {
        // Arrange
        GroupMemberIdentifier first = new("id-123");
        GroupMemberIdentifier second = new("id-123");

        // Act
        int firstHash = first.GetHashCode();
        int secondHash = second.GetHashCode();

        // Assert
        Assert.Equal(firstHash, secondHash);
    }
}
