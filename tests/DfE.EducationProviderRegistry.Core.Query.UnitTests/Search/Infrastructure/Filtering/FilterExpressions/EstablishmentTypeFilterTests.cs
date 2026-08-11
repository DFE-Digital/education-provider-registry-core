using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.FilterExpressions;

public sealed class EstablishmentTypeFilterTests
{
    [Fact]
    public void CreateSpecification_GivenNonNumericValue_ThrowsFormatException()
    {
        // Arrange
        EstablishmentTypeFilter sut = new();

        SearchFilterRequest request = Request(values: ["abc"]);

        // Act / Assert
        Assert.Throws<FormatException>(
            () => sut.CreateSpecification(request));
    }

    [Fact]
    public void CreateSpecification_GivenValueOutsideInt64Range_ThrowsOverflowException()
    {
        // Arrange
        EstablishmentTypeFilter sut = new();

        SearchFilterRequest request =
            Request(values: ["999999999999999999999999999999"]);

        // Act / Assert
        Assert.Throws<OverflowException>(
            () => sut.CreateSpecification(request));
    }

    [Fact]
    public void CreateSpecification_GivenOnlyNullFilterValues_Evaluates_To_True()
    {
        // Arrange
        EstablishmentTypeFilter sut = new();

        SearchFilterRequest request =
            Request(values: [null!]);

        Establishment establishment = new()
        {
            EstablishmentTypeId = 999
        };

        // Act
        ISpecification<Establishment> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(establishment);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CreateSpecification_GivenNullFilterValues_SomeMatchingFields_Evaluates_To_True()
    {
        // Arrange
        EstablishmentTypeFilter sut = new();

        SearchFilterRequest request = Request(values: ["1", null!, "2"]);

        Establishment establishment = new()
        {
            EstablishmentTypeId = 2
        };

        // Act
        ISpecification<Establishment> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(establishment);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CreateSpecification_GivenNullFilterValues_NoMatchingFields_Evaluates_To_False()
    {
        // Arrange
        EstablishmentTypeFilter sut = new();

        SearchFilterRequest request = Request(values: ["1", null!]);
            
        Establishment establishment = new()
        {
            EstablishmentTypeId = 2
        };

        // Act
        ISpecification<Establishment> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(establishment);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CreateSpecification_GivenMatchingEstablishmentTypeId_ReturnsSatisfiedSpecification()
    {
        // Arrange
        SearchFilterRequest request = Request(values: ["1", "2"]);

        Establishment establishment = new()
        {
            EstablishmentTypeId = 2
        };

        EstablishmentTypeFilter sut = new();

        // Act
        ISpecification<Establishment> specification = sut.CreateSpecification(request);

        bool result = specification.IsSatisfiedBy(establishment);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CreateSpecification_GivenNonMatchingEstablishmentTypeId_ReturnsFalseSpecification()
    {
        // Arrange
        SearchFilterRequest request = Request(values: ["1", "2"]);

        Establishment establishment = new()
        {
            EstablishmentTypeId = 3
        };

        EstablishmentTypeFilter sut = new();

        // Act
        ISpecification<Establishment> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(establishment);

        // Assert
        Assert.False(result);
    }

    private static SearchFilterRequest Request(IEnumerable<object> values) => new("STUB-FILTERKEY", values);
}
