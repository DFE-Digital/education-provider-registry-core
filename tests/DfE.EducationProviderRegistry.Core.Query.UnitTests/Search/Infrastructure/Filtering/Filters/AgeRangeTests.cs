using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class AgeRangeTests
{
    [Fact]
    public void Constructor_LowerBoundBelowMinimum_ThrowsInvalidRangeException()
    {
        // Arrange
        BoundedRange<int> boundedRange = new(-1, null);

        // Act
        Func<AgeRange> construct = () => new(boundedRange);

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(construct);

        ValidationErrorCode error = Assert.Single(exception.Errors);
        Assert.Equal(ValidationErrorCode.LowerBeforeMinimum, error);
    }

    [Fact]
    public void Constructor_UpperBoundBelowMinimum_ThrowsInvalidRangeException()
    {
        // Arrange
        BoundedRange<int> boundedRange = new(null, -1);

        // Act
        Func<AgeRange> construct = () => new(boundedRange);

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(construct);

        ValidationErrorCode error = Assert.Single(exception.Errors);
        Assert.Equal(ValidationErrorCode.UpperBeforeMinimum, error);
    }

    [Fact]
    public void Constructor_LowerBoundAboveMaximum_ThrowsInvalidRangeException()
    {
        // Arrange
        BoundedRange<int> boundedRange = new(100, null);

        // Act
        Func<AgeRange> construct = () => new(boundedRange);

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(construct);

        ValidationErrorCode error = Assert.Single(exception.Errors);
        Assert.Equal(ValidationErrorCode.LowerAfterMaximum, error);
    }

    [Fact]
    public void Constructor_UpperBoundAboveMaximum_ThrowsInvalidRangeException()
    {
        // Arrange
        BoundedRange<int> boundedRange = new(null, 100);

        // Act
        Func<AgeRange> construct = () => new(boundedRange);

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(construct);

        ValidationErrorCode error = Assert.Single(exception.Errors);
        Assert.Equal(ValidationErrorCode.UpperAfterMaximum, error);
    }

    [Fact]
    public void Constructor_LowerAndUpperBoundsBelowMinimum_ThrowsInvalidRangeExceptionWithBothErrors()
    {
        // Arrange
        BoundedRange<int> boundedRange = new(-20, -1);

        // Act
        Func<AgeRange> construct = () => new(boundedRange);

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(construct);

        Assert.Equal(2, exception.Errors.Count);

        Assert.Contains(ValidationErrorCode.LowerBeforeMinimum, exception.Errors);
        Assert.Contains(ValidationErrorCode.UpperBeforeMinimum, exception.Errors);
    }

    [Fact]
    public void Constructor_LowerAndUpperBoundsAboveMaximum_ThrowsInvalidRangeExceptionWithBothErrors()
    {
        // Arrange
        BoundedRange<int> boundedRange = new(100, 101);

        // Act
        Func<AgeRange> construct = () => new(boundedRange);

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(construct);

        Assert.Equal(2, exception.Errors.Count);

        Assert.Contains(ValidationErrorCode.LowerAfterMaximum, exception.Errors);
        Assert.Contains(ValidationErrorCode.UpperAfterMaximum, exception.Errors);
    }

    [Fact]
    public void Constructor_LowerBoundEqualToMinimum_CreatesAgeRange()
    {
        // Arrange
        BoundedRange<int> boundedRange = new(0, null);

        // Act
        AgeRange result = new(boundedRange);

        // Assert
        Assert.Equal(boundedRange, result.Range);
    }

    [Fact]
    public void Constructor_UpperBoundEqualToMinimum_CreatesAgeRange()
    {
        // Arrange
        BoundedRange<int> boundedRange = new(null, 0);

        // Act
        AgeRange result = new(boundedRange);

        // Assert
        Assert.Equal(boundedRange, result.Range);
    }

    [Fact]
    public void Constructor_LowerBoundEqualToMaximum_CreatesAgeRange()
    {
        // Arrange
        BoundedRange<int> boundedRange = new(99, null);

        // Act
        AgeRange result = new(boundedRange);

        // Assert
        Assert.Equal(boundedRange, result.Range);
    }

    [Fact]
    public void Constructor_UpperBoundEqualToMaximum_CreatesAgeRange()
    {
        // Arrange
        BoundedRange<int> boundedRange = new(null, 99);

        // Act
        AgeRange result = new(boundedRange);

        // Assert
        Assert.Equal(boundedRange, result.Range);
    }

    [Fact]
    public void Constructor_LowerAndUpperBoundsWithinAllowedRange_CreatesAgeRange()
    {
        // Arrange
        BoundedRange<int> boundedRange = new(5, 16);

        // Act
        AgeRange result = new(boundedRange);

        // Assert
        Assert.Equal(boundedRange, result.Range);
    }
}
