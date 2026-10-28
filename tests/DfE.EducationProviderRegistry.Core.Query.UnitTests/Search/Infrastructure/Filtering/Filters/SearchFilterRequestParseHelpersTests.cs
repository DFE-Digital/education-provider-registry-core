using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class SearchFilterRequestParseHelpersTests
{
    [Fact]
    public void ParseFilterRequestDates_WithMoreThanTwoFilterValues_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                "2020-01-01",
                "2021-01-01",
                "2022-01-01"
            ]);

        // Act
        Func<DateRange> act = () =>
            request.ParseFilterRequestDates();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void ParseFilterRequestDates_WithUnsupportedLowerBoundType_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request = new("key", [1]);

        // Act
        Func<DateRange> act = () =>
            request.ParseFilterRequestDates();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void ParseFilterRequestDates_WithUnsupportedUpperBoundType_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                "2025-10-15",
                1
            ]);

        // Act
        Func<DateRange> act = () =>
            request.ParseFilterRequestDates();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void ParseFilterRequestDates_WithNullLowerAndUpperBounds_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request = new("key", [null!, null!]);

        // Act
        Func<DateRange> act = () =>
            request.ParseFilterRequestDates();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void ParseFilterRequestDates_WithSingleNullValue_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request = new("key", [null!]);

        // Act
        Func<DateRange> act = () =>
            request.ParseFilterRequestDates();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void ParseFilterRequestDates_WithSingleStringValue_ReturnsLowerBound()
    {
        // Arrange
        SearchFilterRequest request = new("key", ["2026-10-15"]);

        // Act
        DateRange result =
            request.ParseFilterRequestDates();

        // Assert
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Lower);
        Assert.Null(result.Range.Upper);
    }

    [Fact]
    public void ParseFilterRequestDates_WithSingleDateTimeValue_ReturnsLowerBound()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                new DateTime(2026, 10, 15, 10, 30, 00, DateTimeKind.Unspecified)
            ]);

        // Act
        DateRange result =
            request.ParseFilterRequestDates();

        // Assert
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Lower);
        Assert.Null(result.Range.Upper);
    }

    [Fact]
    public void ParseFilterRequestDates_WithStringLowerBoundAndNullUpperBound_ReturnsLowerBound()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                "2026-10-15",
                null!
            ]);

        // Act
        DateRange result =
            request.ParseFilterRequestDates();

        // Assert
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Lower);
        Assert.Null(result.Range.Upper);
    }

    [Fact]
    public void ParseFilterRequestDates_WithDateTimeLowerBoundAndNullUpperBound_ReturnsLowerBound()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                new DateTime(2026, 10, 15, 10, 30, 00, DateTimeKind.Unspecified),
                null!
            ]);

        // Act
        DateRange result =
            request.ParseFilterRequestDates();

        // Assert
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Lower);
        Assert.Null(result.Range.Upper);
    }

    [Fact]
    public void ParseFilterRequestDates_WithNullLowerBoundAndStringUpperBound_ReturnsUpperBound()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                null!,
                "2026-10-15"
            ]);

        // Act
        DateRange result =
            request.ParseFilterRequestDates();

        // Assert
        Assert.Null(result.Range.Lower);
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Upper);
    }

    [Fact]
    public void ParseFilterRequestDates_WithNullLowerBoundAndDateTimeUpperBound_ReturnsUpperBound()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                null!,
                new DateTime(2026, 10, 15, 15, 45, 00, DateTimeKind.Unspecified)
            ]);

        // Act
        DateRange result =
            request.ParseFilterRequestDates();

        // Assert
        Assert.Null(result.Range.Lower);
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Upper);
    }

    [Fact]
    public void ParseFilterRequestDates_WithStringLowerAndUpperBounds_ReturnsBothBounds()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                "2025-10-15",
                "2026-10-15"
            ]);

        // Act
        DateRange result =
            request.ParseFilterRequestDates();

        // Assert
        Assert.Equal(new DateOnly(2025, 10, 15), result.Range.Lower);
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Upper);
    }

    [Fact]
    public void ParseFilterRequestDates_WithDateTimeLowerAndUpperBounds_ReturnsBothBounds()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                new DateTime(2025, 10, 15, 10, 30, 00, DateTimeKind.Unspecified),
                new DateTime(2026, 10, 15, 15, 45, 00, DateTimeKind.Unspecified)
            ]);

        // Act
        DateRange result =
            request.ParseFilterRequestDates();

        // Assert
        Assert.Equal(new DateOnly(2025, 10, 15), result.Range.Lower);
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Upper);
    }

    [Fact]
    public void ParseFilterRequestDates_WithStringLowerAndDateTimeUpperBounds_ReturnsBothBounds()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                "2025-10-15",
                new DateTime(2026, 10, 15, 15, 45, 00, DateTimeKind.Unspecified)
            ]);

        // Act
        DateRange result =
            request.ParseFilterRequestDates();

        // Assert
        Assert.Equal(new DateOnly(2025, 10, 15), result.Range.Lower);
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Upper);
    }

    [Fact]
    public void ParseFilterRequestDates_WithDateTimeLowerAndStringUpperBounds_ReturnsBothBounds()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                new DateTime(2025, 10, 15, 10, 30, 00, DateTimeKind.Unspecified),
                "2026-10-15"
            ]);

        // Act
        DateRange result =
            request.ParseFilterRequestDates();

        // Assert
        Assert.Equal(new DateOnly(2025, 10, 15), result.Range.Lower);
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Upper);
    }

    [Theory]
    [InlineData("15-10-2026")]
    [InlineData("15/10/2026")]
    [InlineData("2026/10/15")]
    [InlineData("2026-15-10")]
    [InlineData("2026-02-30")]
    public void ParseFilterRequestDates_WithInvalidLowerBoundFormat_ThrowsFormatException(string lower)
    {
        // Arrange
        SearchFilterRequest request = new("key", [lower]);

        // Act
        Func<DateRange> act = () =>
            request.ParseFilterRequestDates();

        // Assert
        Assert.Throws<FormatException>(act);
    }

    [Theory]
    [InlineData("15-10-2026")]
    [InlineData("15/10/2026")]
    [InlineData("2026/10/15")]
    [InlineData("2026-15-10")]
    [InlineData("2026-02-30")]
    public void ParseFilterRequestDates_WithInvalidUpperBoundFormat_ThrowsFormatException(string upper)
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                "2025-10-15",
                upper
            ]);

        // Act
        Func<DateRange> act = () =>
            request.ParseFilterRequestDates();

        // Assert
        Assert.Throws<FormatException>(act);
    }

    [Fact]
    public void ParseFilterRequestDates_WithLowerBoundAfterUpperBound_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request = new(
            "key",
            [
                "2026-10-15",
                "2025-10-15"
            ]);

        // Act
        Func<DateRange> act = () =>
            request.ParseFilterRequestDates();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }
}
