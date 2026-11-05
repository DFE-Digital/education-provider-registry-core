using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.Filters;

public sealed class DateRangeFilterTests
{
    [Fact]
    public void Constructor_WhenPropertyTargetIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Expression<Func<SearchAggregate, DateOnly?>> propertyTarget = null!;

        // Act
        Func<DateRangeFilter> construct = () => new(propertyTarget);

        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Fact]
    public void CreateSpecification_WhenDateIsEqualToLowerBound_IsSatisfied()
    {
        // Arrange
        DateRangeFilter sut = new(aggregate => aggregate.OpenedDate);

        SearchFilterRequest request = new(
            "key",
            [new DateOnly(2026, 1, 1)]);

        SearchAggregate aggregate = new()
        {
            OpenedDate = new DateOnly(2026, 1, 1)
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        // Assert
        Assert.True(specification.IsSatisfiedBy(aggregate));
    }

    [Fact]
    public void CreateSpecification_WhenDateIsGreaterThanLowerBound_IsSatisfied()
    {
        // Arrange
        DateRangeFilter sut = new(aggregate => aggregate.OpenedDate);

        SearchFilterRequest request = new(
            "key",
            [new DateOnly(2026, 1, 1)]);

        SearchAggregate aggregate = new()
        {
            OpenedDate = new DateOnly(2026, 1, 2)
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        // Assert
        Assert.True(specification.IsSatisfiedBy(aggregate));
    }

    [Fact]
    public void CreateSpecification_WhenDateIsLessThanLowerBound_IsNotSatisfied()
    {
        // Arrange
        DateRangeFilter sut = new(aggregate => aggregate.OpenedDate);

        SearchFilterRequest request = new(
            "key",
            [new DateOnly(2026, 1, 2)]);

        SearchAggregate aggregate = new()
        {
            OpenedDate = new DateOnly(2026, 1, 1)
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        // Assert
        Assert.False(specification.IsSatisfiedBy(aggregate));
    }

    [Fact]
    public void CreateSpecification_WhenDateIsEqualToUpperBound_IsSatisfied()
    {
        // Arrange
        DateRangeFilter sut = new(aggregate => aggregate.OpenedDate);

        SearchFilterRequest request = new(
            "key",
            [null!, new DateOnly(2026, 1, 31)]);

        SearchAggregate aggregate = new()
        {
            OpenedDate = new DateOnly(2026, 1, 31)
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        // Assert
        Assert.True(specification.IsSatisfiedBy(aggregate));
    }

    [Fact]
    public void CreateSpecification_WhenDateIsLessThanUpperBound_IsSatisfied()
    {
        // Arrange
        DateRangeFilter sut = new(aggregate => aggregate.OpenedDate);

        SearchFilterRequest request = new(
            "key",
            [null!, new DateOnly(2026, 1, 31)]);

        SearchAggregate aggregate = new()
        {
            OpenedDate = new DateOnly(2026, 1, 30)
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        // Assert
        Assert.True(specification.IsSatisfiedBy(aggregate));
    }

    [Fact]
    public void CreateSpecification_WhenDateIsGreaterThanUpperBound_IsNotSatisfied()
    {
        // Arrange
        DateRangeFilter sut = new(aggregate => aggregate.OpenedDate);

        SearchFilterRequest request = new(
            "key",
            [null!, new DateOnly(2026, 1, 31)]);

        SearchAggregate aggregate = new()
        {
            OpenedDate = new DateOnly(2026, 2, 1)
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        // Assert
        Assert.False(specification.IsSatisfiedBy(aggregate));
    }

    [Theory]
    [InlineData(2026, 1, 1)]
    [InlineData(2026, 1, 15)]
    [InlineData(2026, 1, 31)]
    public void CreateSpecification_WhenDateIsWithinBothBounds_IsSatisfied(
        int year,
        int month,
        int day)
    {
        // Arrange
        DateRangeFilter sut = new(aggregate => aggregate.OpenedDate);

        SearchFilterRequest request = new(
            "key",
            [
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 31)
            ]);

        SearchAggregate aggregate = new()
        {
            OpenedDate = new DateOnly(year, month, day)
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        // Assert
        Assert.True(specification.IsSatisfiedBy(aggregate));
    }

    [Theory]
    [InlineData(2025, 12, 31)]
    [InlineData(2026, 2, 1)]
    public void CreateSpecification_WhenDateIsOutsideBothBounds_IsNotSatisfied(
        int year,
        int month,
        int day)
    {
        // Arrange
        DateRangeFilter sut = new(aggregate => aggregate.OpenedDate);

        SearchFilterRequest request = new(
            "key",
            [
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 31)
            ]);

        SearchAggregate aggregate = new()
        {
            OpenedDate = new DateOnly(year, month, day)
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        // Assert
        Assert.False(specification.IsSatisfiedBy(aggregate));
    }

    [Fact]
    public void CreateSpecification_WhenPropertyValueIsNull_IsNotSatisfied()
    {
        // Arrange
        DateRangeFilter sut = new(aggregate => aggregate.OpenedDate);

        SearchFilterRequest request = new(
            "key",
            [
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 31)
            ]);

        SearchAggregate aggregate = new()
        {
            OpenedDate = null
        };

        // Act
        ISpecification<SearchAggregate> specification =
            sut.CreateSpecification(request);

        // Assert
        Assert.False(specification.IsSatisfiedBy(aggregate));
    }
}
