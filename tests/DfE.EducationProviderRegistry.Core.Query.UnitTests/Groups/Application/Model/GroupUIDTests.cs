using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Groups.Application.Model;

public sealed class GroupUIDTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WhenGroupIdIsZeroOrNegative_ThrowsArgumentOutOfRangeException(int input)
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentOutOfRangeException>(() => new GroupUID(input));
    }

    [Fact]
    public void Constructor_WhenGroupIdIsPositive_SetsValue()
    {
        // Arrange
        int input = 123;

        // Act
        GroupUID result = new(input);

        // Assert
        Assert.Equal(123, result.Value);
    }

    [Fact]
    public void Equality_WhenValuesAreSame_ShouldBeEqual()
    {
        // Arrange
        GroupUID left = new(123);
        GroupUID right = new(123);

        // Act & Assert
        Assert.Equal(left, right);
        Assert.True(left.Equals(right));
        Assert.True(left == right);
    }

    [Fact]
    public void Equality_WhenValuesDiffer_ShouldNotBeEqual()
    {
        // Arrange
        GroupUID left = new(123);
        GroupUID right = new(456);

        // Act & Assert
        Assert.NotEqual(left, right);
        Assert.False(left.Equals(right));
        Assert.True(left != right);
    }

    [Fact]
    public void Equality_WhenComparedWithDefault_ShouldBehaveCorrectly()
    {
        // Arrange
        GroupUID value = new(123);
        GroupUID defaultValue = default;

        // Act & Assert
        Assert.NotEqual(value, defaultValue);
        Assert.False(value.Equals(defaultValue));
    }

    [Fact]
    public void GetHashCode_WhenEqual_ShouldReturnSameValue()
    {
        // Arrange
        GroupUID left = new(123);
        GroupUID right = new(123);

        // Act
        int leftHash = left.GetHashCode();
        int rightHash = right.GetHashCode();

        // Assert
        Assert.Equal(leftHash, rightHash);
    }
}
