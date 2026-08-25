using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Sort;

public sealed class SortDirectionTests
{
    [Theory]
    [InlineData("asc")]
    [InlineData("ASC")]
    [InlineData("Asc")]
    [InlineData("desc")]
    [InlineData("DESC")]
    [InlineData("Desc")]
    public void Constructor_WithValidInput_ShouldNormalizeAndAssign(string input)
    {
        // act
        SortDirection sortDirection = new(input);

        // Assert
        Assert.Equal(input.ToLowerInvariant(), sortDirection.Value);
    }

    [Fact]
    public void Constructor_WithEmptyInput_ShouldThrowArgumentException()
    {
        // act
        ArgumentException exception =
            Assert.Throws<ArgumentException>(() => new SortDirection(direction: ""));

        // Assert
        Assert.Equal("direction", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullInput_ShouldThrowArgumentNullException()
    {
        // act
        ArgumentException exception =
            Assert.Throws<ArgumentNullException>(() => new SortDirection(direction: null!));

        // Assert
        Assert.Equal("direction", exception.ParamName);
    }

    [Theory]
    [InlineData("up")]
    [InlineData("down")]
    [InlineData("ascending")]
    [InlineData("descending")]
    public void Constructor_WithInvalidInput_ShouldThrowArgumentException(string input)
    {
        // act
        ArgumentException exception =
            Assert.Throws<ArgumentException>(() => new SortDirection(input));

        // Assert
        Assert.Contains($"Unknown sort direction '{input.ToLowerInvariant()}'", exception.Message);
    }

    [Theory]
    [InlineData("asc", true)]
    [InlineData("desc", true)]
    [InlineData("ASC", false)]
    [InlineData("up", false)]
    [InlineData(null, false)]
    public void IsValid_ShouldReturnExpectedResult(string? input, bool expected)
    {
        // act
        bool result = SortDirection.IsValid(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Create_ShouldReturnNormalizedSortDirection()
    {
        // act
        SortDirection sortDirection = SortDirection.Create("DESC");

        // Assert
        Assert.Equal("desc", sortDirection.Value);
    }

    [Fact]
    public void Ascending_ShouldHaveExpectedValue()
    {
        // Act
        SortDirection sortDirection = SortDirection.Ascending;

        // Assert
        Assert.Equal("asc", sortDirection.Value);
    }

    [Fact]
    public void Descending_ShouldHaveExpectedValue()
    {
        // Act
        SortDirection sortDirection = SortDirection.Descending;

        // Assert
        Assert.Equal("desc", sortDirection.Value);
    }

    [Fact]
    public void Ascending_ShouldNotEqualDescending()
    {
        // Assert
        Assert.NotEqual(
            SortDirection.Ascending,
            SortDirection.Descending);
    }
}
