using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters.Decorators;

public sealed class DateOnlyFilterValueSearchFilterDecoratorTests
{
    [Fact]
    public void CreateSpecification_WithDateOnlyValue_PreservesValue()
    {
        // Arrange
        DateOnly expected = new(2026, 10, 15);

        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        DateOnlyFilterValueSearchFilterDecorator<SearchAggregate> sut = new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [expected]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Equal(expected, inner.Request.FilterValues[0]);
    }

    [Fact]
    public void CreateSpecification_WithDateTimeValue_ConvertsToDateOnly()
    {
        // Arrange
        DateTime value =
            new(2026, 10, 15, 10, 30, 0, DateTimeKind.Unspecified);

        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        DateOnlyFilterValueSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [value]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);

        Assert.Equal(
            new DateOnly(2026, 10, 15),
            inner.Request.FilterValues[0]);
    }

    [Fact]
    public void CreateSpecification_WithIsoDateString_ConvertsToDateOnly()
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        DateOnlyFilterValueSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", ["2026-10-15"]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);

        Assert.Equal(
            new DateOnly(2026, 10, 15),
            inner.Request.FilterValues[0]);
    }

    [Fact]
    public void CreateSpecification_WithNullValue_PreservesNull()
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        DateOnlyFilterValueSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [null!]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Null(inner.Request.FilterValues[0]);
    }

    [Theory]
    [InlineData("15-10-2026")]
    [InlineData("15/10/2026")]
    [InlineData("2026/10/15")]
    [InlineData("2026-15-10")]
    [InlineData("2026-02-30")]
    public void CreateSpecification_WithInvalidDateString_PreservesValue(string value)
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        DateOnlyFilterValueSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [value]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Equal(value, inner.Request.FilterValues[0]);
    }

    [Fact]
    public void CreateSpecification_WithUnsupportedValue_PreservesValue()
    {
        // Arrange
        object expected = new();

        FakeSearchFilter<SearchAggregate> inner =
           SearchFilterTestDouble.Fake<SearchAggregate>();

        StringFilterValueSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [expected]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Same(expected, inner.Request.FilterValues[0]);
    }
}
