using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class NumericRangeFilterTests
{
    [Fact]
    public void Constructor_GivenNullLowerPropertyTarget_ThrowsArgumentNullException()
    {
        // Arrange
        Func<NumericRangeFilter> construct = () =>
            new(
                null!,
                x => x.AgeHigh);

        // Act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(construct);

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public void Constructor_GivenNullUpperPropertyTarget_ThrowsArgumentNullException()
    {
        // Arrange
        Func<NumericRangeFilter> construct = () =>
            new(
                x => x.AgeLow,
                null!);

        // Act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(construct);

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public void CreateSpecification_GivenLowerAndUpperBoundsAndProviderWithinRange_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 7,
            AgeHigh = 11
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
    public void CreateSpecification_GivenLowerAndUpperBoundsEqualToProviderBounds_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 5,
            AgeHigh = 16
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
    public void CreateSpecification_GivenLowerAndUpperBoundsAndProviderLowAgeBelowLowerBound_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 4,
            AgeHigh = 11
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
    public void CreateSpecification_GivenLowerAndUpperBoundsAndProviderHighAgeAboveUpperBound_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 5,
            AgeHigh = 17
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
    public void CreateSpecification_GivenOnlyLowerBoundAndProviderLowAgeAboveLowerBound_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, null!]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 7,
            AgeHigh = 11
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
    public void CreateSpecification_GivenOnlyLowerBoundEqualToProviderLowAge_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, null!]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 5,
            AgeHigh = 11
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
    public void CreateSpecification_GivenOnlyLowerBoundAndProviderLowAgeBelowLowerBound_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range",
            [5, null!]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 4,
            AgeHigh = 11
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
    public void CreateSpecification_GivenOnlyUpperBoundAndProviderHighAgeBelowUpperBound_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [null!, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 5,
            AgeHigh = 11
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
    public void CreateSpecification_GivenOnlyUpperBoundEqualToProviderHighAge_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [null!, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 5,
            AgeHigh = 16
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
    public void CreateSpecification_GivenOnlyUpperBoundAndProviderHighAgeAboveUpperBound_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [null!, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 5,
            AgeHigh = 17
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
    public void CreateSpecification_GivenLowerBoundAndProviderLowAgeIsNull_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, null!]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = null,
            AgeHigh = 11
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
    public void CreateSpecification_GivenUpperBoundAndProviderHighAgeIsNull_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [null!, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 5,
            AgeHigh = null
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
    public void CreateSpecification_GivenBothBoundsAndProviderLowAgeIsNull_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = null,
            AgeHigh = 11
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
    public void CreateSpecification_GivenBothBoundsAndProviderHighAgeIsNull_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 7,
            AgeHigh = null
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
