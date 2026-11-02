using DfE.EducationProviderRegistry.Core.Query.Shared.Specifications.Contains;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.Specifications;

public sealed class PropertyContainsAnyValuesSpecificationTests
{
    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenPropertyIsNull()
    {
        // arrange
        Func<PropertyContainsAnyValuesSpecification<TestProjection, string>> construct =
            () => new PropertyContainsAnyValuesSpecification<TestProjection, string>(
                property: null!,
                values: ["value"]);

        // act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(construct);

        // assert
        Assert.NotNull(exception);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenValuesIsNull()
    {
        // arrange
        Func<PropertyContainsAnyValuesSpecification<TestProjection, string>> construct =
            () => new PropertyContainsAnyValuesSpecification<TestProjection, string>(
                projection => projection.Values,
                null!);

        // act
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(construct);

        // assert
        Assert.NotNull(exception);
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenValuesIsEmpty()
    {
        // arrange
        TestProjection input = new()
        {
            Values = ["A", "B"]
        };

        PropertyContainsAnyValuesSpecification<TestProjection, string> sut = new(
            projection => projection.Values,
            []);

        // act
        bool result = sut.IsSatisfiedBy(input);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenPropertyContainsRequestedValue()
    {
        // arrange
        TestProjection input = new()
        {
            Values = ["A", "B", "C"]
        };

        PropertyContainsAnyValuesSpecification<TestProjection, string> sut = new(
            projection => projection.Values,
            ["B"]);

        // act
        bool result = sut.IsSatisfiedBy(input);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenPropertyContainsAnyRequestedValue()
    {
        // arrange
        TestProjection input = new()
        {
            Values = ["A", "B", "C"]
        };

        PropertyContainsAnyValuesSpecification<TestProjection, string> sut = new(
            projection => projection.Values,
            ["X", "B", "Y"]);

        // act
        bool result = sut.IsSatisfiedBy(input);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenPropertyContainsNoneOfRequestedValues()
    {
        // arrange
        TestProjection input = new()
        {
            Values = ["A", "B", "C"]
        };

        PropertyContainsAnyValuesSpecification<TestProjection, string> sut = new(
            projection => projection.Values,
            ["X", "Y", "Z"]);

        // act
        bool result = sut.IsSatisfiedBy(input);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenPropertyCollectionIsEmpty()
    {
        // arrange
        TestProjection input = new()
        {
            Values = []
        };

        PropertyContainsAnyValuesSpecification<TestProjection, string> sut = new(
            projection => projection.Values,
            ["A"]);

        // act
        bool result = sut.IsSatisfiedBy(input);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenPropertyDoesNotContainExactRequestedValue()
    {
        // arrange
        TestProjection input = new()
        {
            Values = ["ASD - Autistic Spectrum Disorder"]
        };

        PropertyContainsAnyValuesSpecification<TestProjection, string> sut = new(
            projection => projection.Values,
            ["ASD"]);

        // act
        bool result = sut.IsSatisfiedBy(input);

        // assert
        Assert.False(result);
    }

    private sealed class TestProjection
    {
        public List<string> Values { get; init; } = [];
    }
}
