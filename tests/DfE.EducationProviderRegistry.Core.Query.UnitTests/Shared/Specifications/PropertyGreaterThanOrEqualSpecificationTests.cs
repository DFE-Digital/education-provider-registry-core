using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Shared.Specifications.Comparison;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.Specifications;

public sealed class PropertyGreaterThanOrEqualSpecificationTests
{
    [Fact]
    public void Constructor_WhenPropertyIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Expression<Func<TestProjection, int?>> property = null!;

        // Act
        Func<PropertyGreaterThanOrEqualSpecification<TestProjection, int>> construct =
            () => new(property, 10);

        // Assert
        Assert.Throws<ArgumentNullException>(construct);
    }

    [Theory]
    [InlineData(11, true)]
    [InlineData(10, true)]
    [InlineData(9, false)]
    [InlineData(null, false)]
    public void IsSatisfiedBy_ReturnsExpectedResult(
        int? value,
        bool expected)
    {
        // Arrange
        PropertyGreaterThanOrEqualSpecification<TestProjection, int> specification =
            new(projection => projection.Value, 10);

        TestProjection input = new()
        {
            Value = value
        };

        // Act
        bool result = specification.IsSatisfiedBy(input);

        // Assert
        Assert.Equal(expected, result);
    }

    private sealed class TestProjection
    {
        public int? Value { get; init; }
    }
}
