using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.FilterExpressions;

public sealed class EstablishmentTypeFilterTests
{
    [Fact]
    public void CreateSpecification_GivenNonNumericValue_ThrowsFormatException()
    {
        // Arrange
        SearchProviderTypeFilter sut = new();

        SearchFilterRequest request = RequestFilterValues(["a"]);

        // Act / Assert
        Assert.Throws<FormatException>(
            () => sut.CreateSpecification(request));
    }

    [Fact]
    public void CreateSpecification_GivenValueOutsideInt64Range_ThrowsOverflowException()
    {
        // Arrange
        SearchProviderTypeFilter sut = new();

        SearchFilterRequest request = RequestFilterValues(["999999999999999999999999999999"]);

        // Act / Assert
        Assert.Throws<OverflowException>(
            () => sut.CreateSpecification(request));
    }

    [Fact]
    public void CreateSpecification_GivenOnlyNullFilterValues_Evaluates_To_True()
    {
        // Arrange
        SearchProviderTypeFilter sut = new();

        SearchFilterRequest request = RequestFilterValues([null!]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeId = 999
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
    public void CreateSpecification_GivenNullFilterValues_SomeMatchingFields_Evaluates_To_True()
    {
        // Arrange
        SearchProviderTypeFilter sut = new();

        SearchFilterRequest request = RequestFilterValues(["1", null!, "2"]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeId = 2
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
    public void CreateSpecification_GivenNullFilterValues_NoMatchingFields_Evaluates_To_False()
    {
        // Arrange
        SearchProviderTypeFilter sut = new();

        SearchFilterRequest request = RequestFilterValues(["1", null!]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeId = 2
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
    public void CreateSpecification_GivenMatchingEstablishmentTypeId_ReturnsSatisfiedSpecification()
    {
        // Arrange
        SearchFilterRequest request = RequestFilterValues(["1", "2"]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeId = 2
        };

        SearchProviderTypeFilter sut = new();

        // Act
        ISpecification<SearchAggregate> specification = sut.CreateSpecification(request);

        bool result = specification.IsSatisfiedBy(searchAggregate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CreateSpecification_GivenNonMatchingEstablishmentTypeId_ReturnsFalseSpecification()
    {
        // Arrange
        SearchFilterRequest request = RequestFilterValues(["1", "2"]);

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeId = 3
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

    private static SearchFilterRequest RequestFilterValues(object[] values) => SearchFilterRequestStub.Create("STUB-FILTERKEY", values);
}
