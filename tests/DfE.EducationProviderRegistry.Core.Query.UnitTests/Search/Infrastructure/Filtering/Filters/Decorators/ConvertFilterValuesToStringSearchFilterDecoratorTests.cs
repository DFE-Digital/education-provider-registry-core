using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Decorators;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters.Decorators;

public sealed class ConvertFilterValuesToStringSearchFilterDecoratorTests
{
    [Theory]
    [InlineData((byte)1, "1")]
    [InlineData((sbyte)1, "1")]
    [InlineData((short)1, "1")]
    [InlineData((ushort)1, "1")]
    [InlineData(1, "1")]
    [InlineData((uint)1, "1")]
    [InlineData(1L, "1")]
    [InlineData((ulong)1, "1")]
    [InlineData(1f, "1")]
    [InlineData(1d, "1")]
    public void CreateSpecification_WithNumericValue_ConvertsToString(
        object value,
        string expected)
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        ConvertFilterValuesToStringSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [value]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Equal(expected, inner.Request.FilterValues[0]);
    }

    [Fact]
    public void CreateSpecification_WithDecimalValue_ConvertsToInvariantString()
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        ConvertFilterValuesToStringSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [1.5m]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Equal("1.5", inner.Request.FilterValues[0]);
    }

    [Fact]
    public void CreateSpecification_WithStringValue_PreservesValue()
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        ConvertFilterValuesToStringSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", ["ProviderType"]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Equal("ProviderType", inner.Request.FilterValues[0]);
    }

    [Fact]
    public void CreateSpecification_WithMixedSupportedValues_ConvertsAllValues()
    {
        // Arrange
        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        ConvertFilterValuesToStringSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", ["1", 2, 3.5m]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);

        Assert.Equal(
            ["1", "2", "3.5"],
            inner.Request.FilterValues);
    }

    [Fact]
    public void CreateSpecification_WithUnsupportedValue_PreservesValue()
    {
        // Arrange
        object value = new();

        FakeSearchFilter<SearchAggregate> inner =
            SearchFilterTestDouble.Fake<SearchAggregate>();

        ConvertFilterValuesToStringSearchFilterDecorator<SearchAggregate> sut =
            new(inner);

        SearchFilterRequest request =
            new("STUB-KEY", [value]);

        // Act
        sut.CreateSpecification(request);

        // Assert
        Assert.NotNull(inner.Request);
        Assert.Same(value, inner.Request.FilterValues[0]);
    }
}
