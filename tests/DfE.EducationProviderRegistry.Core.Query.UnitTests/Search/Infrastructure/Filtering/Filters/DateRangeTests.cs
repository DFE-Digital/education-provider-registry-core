using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class DateRangeTests
{
    [Fact]
    public void Constructor_LowerBoundBeforeMinimum_ThrowsArgumentException()
    {
        // Arrange
        BoundedRange<DateOnly> boundedRange = new(
            new DateOnly(1799, 12, 31),
            null);

        // Act
        Func<DateRange> construct = () => new(boundedRange);

        // Assert
        Assert.Throws<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_UpperBoundBeforeMinimum_ThrowsArgumentException()
    {
        // Arrange
        BoundedRange<DateOnly> boundedRange = new(null, new DateOnly(1799, 12, 31));

        // Act
        Func<DateRange> construct = () => new(boundedRange);

        // Assert
        Assert.Throws<ArgumentException>(construct);
    }

    [Fact]
    public void Constructor_LowerBoundEqualToMinimum_CreatesDateRange()
    {
        // Arrange
        DateOnly lower = new(1800, 1, 1);

        BoundedRange<DateOnly> boundedRange = new(lower, null);

        // Act
        DateRange result = new(boundedRange);

        // Assert
        Assert.Equal(boundedRange, result.Range);
    }

    [Fact]
    public void Constructor_UpperBoundEqualToMinimum_CreatesDateRange()
    {
        // Arrange
        DateOnly upper = new(1800, 1, 1);

        BoundedRange<DateOnly> boundedRange = new(
            null,
            upper);

        // Act
        DateRange result = new(boundedRange);

        // Assert
        Assert.Equal(boundedRange, result.Range);
    }

    [Fact]
    public void Constructor_LowerAndUpperBoundsAfterMinimum_CreatesDateRange()
    {
        // Arrange
        DateOnly lower = new(2020, 1, 1);
        DateOnly upper = new(2025, 1, 1);

        BoundedRange<DateOnly> boundedRange = new(lower, upper);

        // Act
        DateRange result = new(boundedRange);

        // Assert
        Assert.Equal(boundedRange, result.Range);
    }

    [Fact]
    public void Constructor_LowerBoundAfterMinimum_CreatesDateRange()
    {
        // Arrange
        DateOnly lower = new(2020, 1, 1);

        BoundedRange<DateOnly> boundedRange = new(lower, null);

        // Act
        DateRange result = new(boundedRange);

        // Assert
        Assert.Equal(boundedRange, result.Range);
    }

    [Fact]
    public void Constructor_UpperBoundAfterMinimum_CreatesDateRange()
    {
        // Arrange
        DateOnly upper = new(2025, 1, 1);

        BoundedRange<DateOnly> boundedRange = new(null, upper);

        // Act
        DateRange result = new(boundedRange);

        // Assert
        Assert.Equal(boundedRange, result.Range);
    }
}
