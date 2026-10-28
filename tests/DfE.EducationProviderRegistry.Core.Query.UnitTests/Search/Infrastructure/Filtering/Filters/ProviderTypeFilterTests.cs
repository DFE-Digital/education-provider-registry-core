using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class SearchProviderTypeFilterTests
{
    [Fact]
    public void CreateSpecification_GivenOnlyNullFilterValues_EvaluatesToTrue()
    {
        // Arrange
        SearchProviderTypeFilter sut = new();

        SearchFilterRequest request = RequestFilterValues([null!]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeName = "999"
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(searchAggregate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CreateSpecification_GivenNullFilterValuesAndMatchingProviderType_EvaluatesToTrue()
    {
        // Arrange
        SearchProviderTypeFilter sut = new();

        SearchFilterRequest request = RequestFilterValues(["1", null!, "2"]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeName = "2"
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(searchAggregate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CreateSpecification_GivenNullFilterValuesAndNoMatchingProviderType_EvaluatesToFalse()
    {
        // Arrange
        SearchProviderTypeFilter sut = new();

        SearchFilterRequest request = RequestFilterValues(["1", null!]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeName = "2"
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(searchAggregate);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CreateSpecification_GivenMatchingProviderType_EvaluatesToTrue()
    {
        // Arrange
        SearchFilterRequest request = RequestFilterValues(["1", "2"]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeName = "2"
        };

        SearchProviderTypeFilter sut = new();

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(searchAggregate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CreateSpecification_GivenNonMatchingProviderType_EvaluatesToFalse()
    {
        // Arrange
        SearchFilterRequest request = RequestFilterValues(["1", "2"]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeName = "3"
        };

        SearchProviderTypeFilter sut = new();

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(searchAggregate);

        // Assert
        Assert.False(result);
    }

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
    public void CreateSpecification_GivenSupportedNumericValue_EvaluatesToTrue(
        object filterValue,
        string providerType)
    {
        // Arrange
        SearchFilterRequest request = RequestFilterValues([filterValue]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeName = providerType
        };

        SearchProviderTypeFilter sut = new();

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(searchAggregate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CreateSpecification_GivenUnsupportedFilterValue_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request =
            RequestFilterValues([new object()]);

        SearchProviderTypeFilter sut = new();

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => sut.CreateSpecification(request));
    }

    [Fact]
    public void CreateSpecification_GivenMixedSupportedAndUnsupportedValues_ThrowsArgumentException()
    {
        // Arrange
        SearchFilterRequest request =
            RequestFilterValues(["1", 2, new object()]);

        SearchProviderTypeFilter sut = new();

        // Act & Assert
        Assert.Throws<ArgumentException>(
            () => sut.CreateSpecification(request));
    }

    [Fact]
    public void CreateSpecification_GivenStringAndNumericFilterValuesAndMatchingProviderType_EvaluatesToTrue()
    {
        // Arrange
        SearchFilterRequest request =
            RequestFilterValues(["1", 2, "3"]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeName = "2"
        };

        SearchProviderTypeFilter sut = new();

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(searchAggregate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CreateSpecification_GivenDecimalFilterValueAndMatchingProviderType_EvaluatesToTrue()
    {
        // Arrange
        SearchFilterRequest request =
            RequestFilterValues([1.5m]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeName = "1.5"
        };

        SearchProviderTypeFilter sut = new();

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(searchAggregate);

        // Assert
        Assert.True(result);
    }

    private static SearchFilterRequest RequestFilterValues(object[] values) =>
        SearchFilterRequestStub.Create("STUB-FILTERKEY", values);
}
