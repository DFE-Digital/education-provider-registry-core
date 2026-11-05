using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class SearchFilterRequestParseHelpersTests
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
    public void ParseDateRange_WithUnsupportedLowerBoundType_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(1);

        // Act
        Func<DateRange> act = () =>
            request.ParseDateRange();

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void ParseDateRange_WithUnsupportedUpperBoundType_ThrowsArgumentException()
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
        Assert.Throws<ArgumentException>(act);
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
        SearchFilterRequest request =
            new(filterKey: "stub", [null!]);

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
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                new DateOnly(2026, 10, 15));

        // Act
        DateRange result =
            request.ParseDateRange();

        // Assert
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Lower);
        Assert.Null(result.Range.Upper);
    }

    [Fact]
    public void ParseDateRange_WithDateOnlyLowerBoundAndNullUpperBound_ReturnsLowerBound()
    {
        // Arrange
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                new DateOnly(2026, 10, 15),
                null!);

        // Act
        DateRange result =
            request.ParseDateRange();

        // Assert
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Lower);
        Assert.Null(result.Range.Upper);
    }

    [Fact]
    public void ParseDateRange_WithNullLowerBoundAndDateOnlyUpperBound_ReturnsUpperBound()
    {
        // Arrange
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                null!,
                new DateOnly(2026, 10, 15));

        // Act
        DateRange result =
            request.ParseDateRange();

        // Assert
        Assert.Null(result.Range.Lower);
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Upper);
    }

    [Fact]
    public void ParseDateRange_WithDateOnlyLowerAndUpperBounds_ReturnsBothBounds()
    {
        // Arrange
        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues(
                new DateOnly(2025, 10, 15),
                new DateOnly(2026, 10, 15));

        // Act
        DateRange result =
            request.ParseDateRange();

        // Assert
        Assert.Equal(new DateOnly(2025, 10, 15), result.Range.Lower);
        Assert.Equal(new DateOnly(2026, 10, 15), result.Range.Upper);
    }

    [Fact]
    public void ParseDateRange_WithLowerBoundAfterUpperBound_ThrowsArgumentException()
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
        Assert.Throws<ArgumentException>(act);
    }
}
