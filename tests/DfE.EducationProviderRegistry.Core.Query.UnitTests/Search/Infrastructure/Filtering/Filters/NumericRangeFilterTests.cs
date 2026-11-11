using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters.Exceptions;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class NumericRangeFilterTests
{
    [Fact]
    public void Constructor_GivenNullLowerPropertyTarget_ThrowsArgumentNullException()
    {
        // Arrange
        Func<NumericRangeFilter<SearchAggregate>> construct = () =>
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
        Func<NumericRangeFilter<SearchAggregate>> construct = () =>
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
    public void CreateSpecification_GivenLowerBoundBeforeMinimum_ThrowsInvalidRangeException()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [-1, null!]);

        // Act
        Func<ISpecification<SearchAggregate>> act =
            () => sut.CreateSpecification(request);

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(act);

        ValidationErrorCode error = Assert.Single(exception.Errors);
        Assert.Equal(ValidationErrorCode.LowerBeforeMinimum, error);
    }

    [Fact]
    public void CreateSpecification_GivenUpperBoundBeforeMinimum_ThrowsInvalidRangeException()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [null!, -1]);

        // Act
        Func<ISpecification<SearchAggregate>> act =
            () => sut.CreateSpecification(request);

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(act);

        ValidationErrorCode error = Assert.Single(exception.Errors);
        Assert.Equal(ValidationErrorCode.UpperBeforeMinimum, error);
    }

    [Fact]
    public void CreateSpecification_GivenLowerBoundAfterMaximum_ThrowsInvalidRangeException()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [100, null!]);

        // Act
        Func<ISpecification<SearchAggregate>> act =
            () => sut.CreateSpecification(request);

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(act);

        ValidationErrorCode error = Assert.Single(exception.Errors);
        Assert.Equal(ValidationErrorCode.LowerAfterMaximum, error);
    }

    [Fact]
    public void CreateSpecification_GivenUpperBoundAfterMaximum_ThrowsInvalidRangeException()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [null!, 100]);

        // Act
        Func<ISpecification<SearchAggregate>> act =
            () => sut.CreateSpecification(request);

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(act);

        ValidationErrorCode error = Assert.Single(exception.Errors);
        Assert.Equal(ValidationErrorCode.UpperAfterMaximum, error);
    }

    [Fact]
    public void CreateSpecification_GivenLowerBoundAfterUpperBound_ThrowsInvalidRangeException()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [15, 9]);

        // Act
        Func<ISpecification<SearchAggregate>> act =
            () => sut.CreateSpecification(request);

        // Assert
        InvalidRangeException exception =
            Assert.Throws<InvalidRangeException>(act);

        ValidationErrorCode error = Assert.Single(exception.Errors);
        Assert.Equal(ValidationErrorCode.UpperNotGreaterThanLower, error);
    }

    [Fact]
    public void CreateSpecification_GivenMinimumAge_CreatesSpecification()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [0, null!]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 0,
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
    public void CreateSpecification_GivenMaximumAge_CreatesSpecification()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [null!, 99]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 16,
            AgeHigh = 99
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
    public void CreateSpecification_GivenLowerAndUpperBoundsAndProviderCoversRange_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 4,
            AgeHigh = 17
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
        NumericRangeFilter<SearchAggregate> sut = new(
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
    public void CreateSpecification_GivenLowerAndUpperBoundsAndProviderDoesNotCoverLowerBound_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 6,
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
    public void CreateSpecification_GivenLowerAndUpperBoundsAndProviderDoesNotCoverUpperBound_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 4,
            AgeHigh = 15
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
    public void CreateSpecification_GivenOnlyLowerBoundAndProviderCoversRequestedAge_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, null!]);

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
        Assert.True(result);
    }

    [Fact]
    public void CreateSpecification_GivenOnlyLowerBoundEqualToProviderLowAge_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
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
    public void CreateSpecification_GivenOnlyLowerBoundEqualToProviderHighAge_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, null!]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 2,
            AgeHigh = 5
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
    public void CreateSpecification_GivenOnlyLowerBoundAndProviderStartsAboveRequestedAge_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, null!]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 6,
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
    public void CreateSpecification_GivenOnlyLowerBoundAndProviderEndsBelowRequestedAge_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, null!]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 2,
            AgeHigh = 4
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
    public void CreateSpecification_GivenOnlyUpperBoundAndProviderCoversRequestedAge_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
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
        Assert.True(result);
    }

    [Fact]
    public void CreateSpecification_GivenOnlyUpperBoundEqualToProviderHighAge_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
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
    public void CreateSpecification_GivenOnlyUpperBoundEqualToProviderLowAge_EvaluatesToTrue()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [null!, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 16,
            AgeHigh = 18
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
    public void CreateSpecification_GivenOnlyUpperBoundAndProviderStartsAboveRequestedAge_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [null!, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 17,
            AgeHigh = 18
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
    public void CreateSpecification_GivenOnlyUpperBoundAndProviderEndsBelowRequestedAge_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [null!, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 5,
            AgeHigh = 15
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
        NumericRangeFilter<SearchAggregate> sut = new(
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
    public void CreateSpecification_GivenLowerBoundAndProviderHighAgeIsNull_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, null!]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 4,
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
    public void CreateSpecification_GivenUpperBoundAndProviderLowAgeIsNull_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [null!, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = null,
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
    public void CreateSpecification_GivenUpperBoundAndProviderHighAgeIsNull_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
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
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = null,
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
    public void CreateSpecification_GivenBothBoundsAndProviderHighAgeIsNull_EvaluatesToFalse()
    {
        // Arrange
        NumericRangeFilter<SearchAggregate> sut = new(
            x => x.AgeLow,
            x => x.AgeHigh);

        SearchFilterRequest request = new(
            "age_range", [5, 16]);

        SearchAggregate searchAggregate = new()
        {
            AgeLow = 4,
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
