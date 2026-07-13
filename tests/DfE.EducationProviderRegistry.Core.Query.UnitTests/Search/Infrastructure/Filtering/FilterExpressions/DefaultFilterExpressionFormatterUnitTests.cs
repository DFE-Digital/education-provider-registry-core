using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Formatters;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.FilterExpressions;

public sealed class DefaultFilterExpressionFormatterUnitTests
{
    private static DefaultFilterExpressionFormatter CreateFormatter() => new();

    [Fact]
    public void CreateFilterCriteriaPlaceholders_NoSeparator_ReturnsCorrectPlaceholders()
    {
        // arrange
        DefaultFilterExpressionFormatter formatter = CreateFormatter();

        // act
        string result = formatter.CreateFilterCriteriaPlaceholders(["A", "B", "C"]);

        // assert
        Assert.Equal("{0}{1}{2}", result);
    }

    [Fact]
    public void CreateFilterCriteriaPlaceholders_WithSeparator_ReturnsCorrectPlaceholders()
    {
        // arrange
        DefaultFilterExpressionFormatter formatter = CreateFormatter();
        formatter.SetExpressionParamsSeparator(",");

        // act
        string result = formatter.CreateFilterCriteriaPlaceholders(["A", "B", "C"]);

        // assert
        Assert.Equal("{0},{1},{2}", result);
    }

    [Fact]
    public void CreateFilterCriteriaPlaceholders_EmptyCriteria_ReturnsEmptyString()
    {
        // arrange
        DefaultFilterExpressionFormatter formatter = CreateFormatter();

        // act
        string result = formatter.CreateFilterCriteriaPlaceholders([]);

        // assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void CreateFormattedExpression_ValidInputs_ReturnsFormattedString()
    {
        // arrange
        DefaultFilterExpressionFormatter formatter = CreateFormatter();

        // act
        string result = formatter.CreateFormattedExpression(
            "({0} AND {1})",
            "A",
            "B");

        // assert
        Assert.Equal("(A AND B)", result);
    }

    [Fact]
    public void CreateFormattedExpression_ThrowsForNullExpressionFormat()
    {
        // arrange
        DefaultFilterExpressionFormatter formatter = CreateFormatter();

        // assert
        Assert.Throws<ArgumentNullException>(() =>
            formatter.CreateFormattedExpression(null!, "A"));
    }

    [Fact]
    public void CreateFormattedExpression_ThrowsForWhitespaceExpressionFormat()
    {
        // arrange
        DefaultFilterExpressionFormatter formatter = CreateFormatter();

        // assert
        Assert.Throws<ArgumentException>(() =>
            formatter.CreateFormattedExpression("   ", "A"));
    }

    [Fact]
    public void CreateFormattedExpression_ThrowsForEmptyCriteria()
    {
        // arrange
        DefaultFilterExpressionFormatter formatter = CreateFormatter();

        // assert
        Assert.Throws<ArgumentException>(() =>
            formatter.CreateFormattedExpression("{0}", []));
    }

    [Fact]
    public void CreateFormattedExpression_RespectsSeparatorInPlaceholderGeneration()
    {
        // arrange
        DefaultFilterExpressionFormatter formatter = CreateFormatter();
        formatter.SetExpressionParamsSeparator(",");

        // act
        string placeholders = formatter.CreateFilterCriteriaPlaceholders(["A", "B"]);

        string result = formatter.CreateFormattedExpression(
            $"search.in({placeholders})",
            "A",
            "B");

        // assert
        Assert.Equal("search.in(A,B)", result);
    }
}
