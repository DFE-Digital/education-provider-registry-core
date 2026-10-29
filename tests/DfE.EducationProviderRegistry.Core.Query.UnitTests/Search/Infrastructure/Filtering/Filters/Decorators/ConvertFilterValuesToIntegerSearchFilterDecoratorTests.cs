using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters.Decorators;

public sealed class ConvertFilterValuesToIntegerSearchFilterDecoratorTests
{
    [Fact]
    public void CreateSpecification_WithIntegerValue_PreservesValue()
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        ConvertFilterValuesToIntegerSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [123]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Equal(123, inner.Request.FilterValues[0]);
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("123", 123)]
    [InlineData("-123", -123)]
    public void CreateSpecification_WithIntegerString_ConvertsToInteger(
        string value,
        int expected)
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        ConvertFilterValuesToIntegerSearchFilterDecorator<SearchAggregate> sut =
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
    public void CreateSpecification_WithInvalidIntegerString_PreservesValue(
        string value)
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        ConvertFilterValuesToIntegerSearchFilterDecorator<SearchAggregate> sut =
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

        ConvertFilterValuesToIntegerSearchFilterDecorator<SearchAggregate> sut =
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
