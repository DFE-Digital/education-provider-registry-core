using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class BoundedRangeTests
{
    [Fact]
    public void Constructor_ThrowsArgumentException_WhenLowerAndUpperAreNull()
    {
        // Act + Assert
        Assert.Throws<ArgumentException>(() => new BoundedRange<int>(null, null!));
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenLowerIsGreaterThanUpper()
    {
        // Act + Assert
        Assert.Throws<ArgumentException>(() => new BoundedRange<int>(10, 5));
    }

    [Fact]
    public void Constructor_SetsLowerAndUpper_WhenBothBoundsHaveValues()
    {
        // Arrange
        const int lower = 5;
        const int upper = 10;

        // Act
        BoundedRange<int> range = new(lower, upper);

        // Assert
        Assert.Equal(lower, range.Lower);
        Assert.Equal(upper, range.Upper);
        Assert.True(range.HasLowerBounds);
        Assert.True(range.HasUpperBounds);
        Assert.True(range.HasLowerAndUpperBounds);
    }

    [Fact]
    public void Constructor_AllowsEqualLowerAndUpperBounds()
    {
        // Arrange
        const int value = 5;

        // Act
        BoundedRange<int> range = new(value, value);

        // Assert
        Assert.Equal(value, range.Lower);
        Assert.Equal(value, range.Upper);
        Assert.True(range.HasLowerAndUpperBounds);
    }

    [Fact]
    public void Constructor_SetsOnlyLower_WhenUpperIsNull()
    {
        // Arrange
        const int lower = 5;

        // Act
        BoundedRange<int> range = new(lower, null);

        // Assert
        Assert.Equal(lower, range.Lower);
        Assert.Null(range.Upper);
        Assert.True(range.HasLowerBounds);
        Assert.False(range.HasUpperBounds);
        Assert.False(range.HasLowerAndUpperBounds);
    }

    [Fact]
    public void Constructor_SetsOnlyUpper_WhenLowerIsNull()
    {
        // Arrange
        const int upper = 10;

        // Act
        BoundedRange<int> range = new(null, upper);

        // Assert
        Assert.Null(range.Lower);
        Assert.Equal(upper, range.Upper);
        Assert.False(range.HasLowerBounds);
        Assert.True(range.HasUpperBounds);
        Assert.False(range.HasLowerAndUpperBounds);
    }
}
