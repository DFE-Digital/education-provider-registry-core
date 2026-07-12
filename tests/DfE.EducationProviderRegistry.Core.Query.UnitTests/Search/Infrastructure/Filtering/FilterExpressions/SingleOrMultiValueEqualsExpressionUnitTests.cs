using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.FilterExpressions;

public sealed class SingleOrMultiValueEqualsExpressionUnitTests
{
    private static SearchFilterRequest Req(string key, params object[] values) => new(key, values);

    [Fact]
    public void GetFilterExpression_Throws_WhenRequestIsNull()
    {
        // arrange
        SingleOrMultiValueEqualsExpression expression = new();

        // act // assert
        Assert.Throws<ArgumentNullException>(() => expression.GetFilterExpression(null!));
    }

    [Fact]
    public void GetFilterExpression_ReturnsEmpty_WhenAllValuesAreIgnored()
    {
        // arrange
        SingleOrMultiValueEqualsExpression expression = new();
        SearchFilterRequest request = Req("col", null!, "", "   ");

        // act
        string result = expression.GetFilterExpression(request);

        // assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetFilterExpression_ReturnsSingleEquality_WhenOneValue()
    {
        // arrange
        SingleOrMultiValueEqualsExpression expression = new();
        SearchFilterRequest request = Req("col", "abc");

        // act
        string result = expression.GetFilterExpression(request);

        // assert
        Assert.Equal("col = 'abc'", result);
    }

    [Fact]
    public void GetFilterExpression_ReturnsOrChain_WhenMultipleValues()
    {
        // arrange
        SingleOrMultiValueEqualsExpression expression = new();
        SearchFilterRequest request = Req("col", "a", "b", "c");

        // act
        string result = expression.GetFilterExpression(request);

        // assert
        Assert.Equal("(col = 'a' OR col = 'b' OR col = 'c')", result);
    }

    [Fact]
    public void GetFilterExpression_IgnoresNullAndEmptyValues()
    {
        // arrange
        SingleOrMultiValueEqualsExpression expression = new();
        SearchFilterRequest request = Req("col", "a", null!, "", "b");

        // act
        string result = expression.GetFilterExpression(request);

        // assert
        Assert.Equal("(col = 'a' OR col = 'b')", result);
    }

    [Fact]
    public void GetFilterExpression_EscapesSingleQuotes()
    {
        // arrange
        SingleOrMultiValueEqualsExpression expression = new();
        SearchFilterRequest request = Req("col", "O'Malley");

        // act
        string result = expression.GetFilterExpression(request);

        // assert
        Assert.Equal("col = 'O''Malley'", result);
    }

    [Fact]
    public void GetFilterExpression_FormatsBooleanValues()
    {
        // arrange
        SingleOrMultiValueEqualsExpression expression = new();
        SearchFilterRequest requestTrue = Req("col", true);
        SearchFilterRequest requestFalse = Req("col", false);

        // act
        string resultTrue = expression.GetFilterExpression(requestTrue);
        string resultFalse = expression.GetFilterExpression(requestFalse);

        // assert
        Assert.Equal("col = TRUE", resultTrue);
        Assert.Equal("col = FALSE", resultFalse);
    }

    [Fact]
    public void GetFilterExpression_FormatsNonStringValues()
    {
        // arrange
        SingleOrMultiValueEqualsExpression expression = new();
        SearchFilterRequest request = Req("col", 123);

        // act
        string result = expression.GetFilterExpression(request);

        // assert
        Assert.Equal("col = 123", result);
    }

    [Fact]
    public void GetFilterExpression_IgnoresNonStringEmptyValues()
    {
        // arrange
        SingleOrMultiValueEqualsExpression expression = new();
        SearchFilterRequest request = Req("col", 123, "", "x");

        // act
        string result = expression.GetFilterExpression(request);

        // assert
        Assert.Equal("(col = 123 OR col = 'x')", result);
    }
}
