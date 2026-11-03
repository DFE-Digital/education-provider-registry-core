using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class EqualsFilterTests
{
    [Fact]
    public void Constructor_GivenNullPropertySelector_ThrowsArgumentNullException()
    {
        // Arrange
        Func<EqualsFilter<SearchAggregate>> construct = () => new(null!);

        // Act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(construct);

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public void CreateSpecification_GivenOnlyNullFilterValues_EvaluatesToFalse()
    {
        // Arrange
        EqualsFilter<SearchAggregate> sut = new(x => x.ProviderTypeName);

        SearchFilterRequest request = new("STUB-KEY", [null!]);

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
        Assert.False(result);
    }

    [Fact]
    public void CreateSpecification_GivenNullFilterValuesAndMatchingProviderType_EvaluatesToTrue()
    {
        // Arrange
        EqualsFilter<SearchAggregate> sut = new(x => x.ProviderTypeName);

        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues("1", null!, "2");

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
        EqualsFilter<SearchAggregate> sut = new(x => x.ProviderTypeName);

        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues("1", null!);

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
        EqualsFilter<SearchAggregate> sut = new(x => x.ProviderTypeName);

        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues("1", "2");

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
    public void CreateSpecification_GivenNonMatchingProviderType_EvaluatesToFalse()
    {
        // Arrange
        EqualsFilter<SearchAggregate> sut = new(x => x.ProviderTypeName);

        SearchFilterRequest request =
            SearchFilterRequestTestDoubles.StubFilterValues("1", "2");

        SearchAggregate searchAggregate = new()
        {
            ProviderTypeName = "3"
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        bool result =
            specification.IsSatisfiedBy(searchAggregate);

        // Assert
        Assert.False(result);
    }
}
