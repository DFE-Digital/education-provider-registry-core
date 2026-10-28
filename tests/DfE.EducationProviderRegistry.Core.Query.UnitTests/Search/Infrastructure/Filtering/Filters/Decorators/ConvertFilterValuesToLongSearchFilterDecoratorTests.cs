using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters.Decorators;

public sealed class LongFilterValueSearchFilterDecoratorTests
{
    [Fact]
    public void CreateSpecification_WithLongValue_PreservesValue()
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        LongFilterValueSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [123L]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Equal(123L, inner.Request.FilterValues[0]);
    }

    [Theory]
    [InlineData("0", 0L)]
    [InlineData("123", 123L)]
    [InlineData("-123", -123L)]
    [InlineData("9223372036854775807", long.MaxValue)]
    [InlineData("-9223372036854775808", long.MinValue)]
    public void CreateSpecification_WithLongString_ConvertsToLong(
        string value,
        long expected)
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        LongFilterValueSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [value]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Equal(expected, inner.Request.FilterValues[0]);
    }

    [Theory]
    [InlineData("1.5")]
    [InlineData("abc")]
    [InlineData("9223372036854775808")]
    [InlineData("-9223372036854775809")]
    public void CreateSpecification_WithInvalidLongString_PreservesValue(
        string value)
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        LongFilterValueSearchFilterDecorator<SearchAggregate> sut =
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
        decimal value = 1.5m;

        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        LongFilterValueSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [value]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Equal(value, inner.Request.FilterValues[0]);
    }
}
