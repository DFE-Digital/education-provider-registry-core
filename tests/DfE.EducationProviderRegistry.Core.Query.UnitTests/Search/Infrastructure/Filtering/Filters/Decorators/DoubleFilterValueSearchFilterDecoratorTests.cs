using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters.Decorators;

public sealed class DoubleFilterValueSearchFilterDecoratorTests
{
    [Fact]
    public void CreateSpecification_WithDoubleValue_PreservesValue()
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        DoubleFilterValueSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [1.5d]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Equal(1.5d, inner.Request.FilterValues[0]);
    }

    [Theory]
    [InlineData("0", 0d)]
    [InlineData("1.5", 1.5d)]
    [InlineData("-1.5", -1.5d)]
    [InlineData("123.456", 123.456d)]
    [InlineData("1E+10", 1E+10d)]
    public void CreateSpecification_WithDoubleString_ConvertsToDouble(
        string value,
        double expected)
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        DoubleFilterValueSearchFilterDecorator<SearchAggregate> sut =
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
    [InlineData("abc")]
    [InlineData("1,5")]
    public void CreateSpecification_WithInvalidDoubleString_PreservesValue(
        string value)
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        DoubleFilterValueSearchFilterDecorator<SearchAggregate> sut =
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

        DoubleFilterValueSearchFilterDecorator<SearchAggregate> sut =
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
