using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Extensions;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class SearchFilterRequestParseExtensionTests
{
    [Fact]
    public void ParseDateRange_WithMoreThanTwoFilterValues_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                new DateOnly(2020, 1, 1),
                new DateOnly(2021, 1, 1),
                new DateOnly(2022, 1, 1));

        // Act
        Func<DateRange> act = () =>
            request.ParseDateRange();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void ParseDateRange_WithUnsupportedLowerBoundType_ThrowsInvalidRangeException()
    {
        // Arrange
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(1);

        // Act
        Func<DateRange> act = () =>
            request.ParseDateRange();

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(act);

        ValidationErrorCode error =
            Assert.Single(exception.Errors);

        Assert.Equal(
            ValidationErrorCode.LowerNotValid,
            error);
    }

    [Fact]
    public void ParseDateRange_WithUnsupportedUpperBoundType_ThrowsInvalidRangeException()
    {
        // Arrange
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                new DateOnly(2025, 10, 15),
                1);

        // Act
        Func<DateRange> act = () =>
            request.ParseDateRange();

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(act);

        ValidationErrorCode error =
            Assert.Single(exception.Errors);

        Assert.Equal(
            ValidationErrorCode.UpperNotValid,
            error);
    }

    [Fact]
    public void ParseDateRange_WithNullLowerAndUpperBounds_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                null!,
                null!);

        // Act
        Func<DateRange> act = () =>
            request.ParseDateRange();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void ParseDateRange_WithSingleNullValue_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request = new("stub-key", [null!]);

        // Act
        Func<DateRange> act = () =>
            request.ParseDateRange();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void ParseDateRange_WithSingleDateOnlyValue_ReturnsLowerBound()
    {
        // Arrange
        DateOnly lower = new(2026, 10, 15);

        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(lower);

        // Act
        DateRange result =
            request.ParseDateRange();

        // Assert
        Assert.Equal(lower, result.Range.Lower);
        Assert.Null(result.Range.Upper);
    }

    [Fact]
    public void ParseDateRange_WithDateOnlyLowerBoundAndNullUpperBound_ReturnsLowerBound()
    {
        // Arrange
        DateOnly lower = new(2026, 10, 15);

        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                lower,
                null!);

        // Act
        DateRange result =
            request.ParseDateRange();

        // Assert
        Assert.Equal(lower, result.Range.Lower);
        Assert.Null(result.Range.Upper);
    }

    [Fact]
    public void ParseDateRange_WithNullLowerBoundAndDateOnlyUpperBound_ReturnsUpperBound()
    {
        // Arrange
        DateOnly upper = new(2026, 10, 15);

        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                null!,
                upper);

        // Act
        DateRange result =
            request.ParseDateRange();

        // Assert
        Assert.Null(result.Range.Lower);
        Assert.Equal(upper, result.Range.Upper);
    }

    [Fact]
    public void ParseDateRange_WithDateOnlyLowerAndUpperBounds_ReturnsBothBounds()
    {
        // Arrange
        DateOnly lower = new(2025, 10, 15);
        DateOnly upper = new(2026, 10, 15);

        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                lower,
                upper);

        // Act
        DateRange result =
            request.ParseDateRange();

        // Assert
        Assert.Equal(lower, result.Range.Lower);
        Assert.Equal(upper, result.Range.Upper);
    }

    [Fact]
    public void ParseDateRange_WithLowerBoundAfterUpperBound_ThrowsInvalidRangeException()
    {
        // Arrange
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                new DateOnly(2026, 10, 15),
                new DateOnly(2025, 10, 15));

        // Act
        Func<DateRange> act = () =>
            request.ParseDateRange();

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(act);

        ValidationErrorCode error =
            Assert.Single(exception.Errors);

        Assert.Equal(
            ValidationErrorCode.UpperNotGreaterThanLower,
            error);
    }
    [Fact]
    public void ParseDateRange_WithInvalidLowerDateString_ThrowsLowerNotValid()
    {
        // Arrange
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                "2026--",
                null!);

        // Act
        Func<DateRange> act = () =>
            request.ParseDateRange();

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(act);

        ValidationErrorCode error =
            Assert.Single(exception.Errors);

        Assert.Equal(
            ValidationErrorCode.LowerNotValid,
            error);
    }

    [Fact]
    public void ParseDateRange_WithInvalidUpperDateString_ThrowsUpperNotValid()
    {
        // Arrange
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                null!,
                "2026--");

        // Act
        Func<DateRange> act = () =>
            request.ParseDateRange();

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(act);

        ValidationErrorCode error =
            Assert.Single(exception.Errors);

        Assert.Equal(
            ValidationErrorCode.UpperNotValid,
            error);
    }
}
