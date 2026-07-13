using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering;

public sealed class SearchFilterRequestUnitTests
{
    [Fact]
    public void Constructor_WithValidArguments_SetsPropertiesCorrectly()
    {
        // arrange
        string key = "CODE";
        object[] values = ["A", "B"];

        // act
        SearchFilterRequest request = new(key, values);

        // assert
        Assert.Equal(key, request.FilterKey);
        Assert.Equal(values, request.FilterValues);
        Assert.Equal(string.Empty, request.FilterValuesDelimiter);
    }

    [Fact]
    public void Constructor_CopiesFilterValuesIntoNewArray()
    {
        // arrange
        object[] original = ["X", "Y"];

        // act
        SearchFilterRequest request = new("KEY", original);

        // mutate original
        original[0] = "CHANGED";

        // assert
        Assert.Equal("X", request.FilterValues[0]);
    }

    [Fact]
    public void Constructor_WithNullFilterKey_ThrowsArgumentException()
    {
        // arrange
        object[] values = ["A"];

        // act
        ArgumentException ex = Assert.Throws<ArgumentNullException>(
            () => new SearchFilterRequest(null!, values));

        // assert
        Assert.Equal("filterKey", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithEmptyFilterKey_ThrowsArgumentException()
    {
        // arrange
        object[] values = ["A"];

        // act
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => new SearchFilterRequest(string.Empty, values));

        // assert
        Assert.Equal("filterKey", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullFilterValues_ThrowsArgumentNullException()
    {
        // act
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
            () => new SearchFilterRequest("KEY", null!));

        // assert
        Assert.Equal("filterValues", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithEmptyFilterValues_ThrowsArgumentException()
    {
        // arrange
        object[] empty = [];

        // act
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => new SearchFilterRequest("KEY", empty));

        // assert
        Assert.Equal("filterValues", ex.ParamName);
    }

    [Fact]
    public void SetFilterValuesDelimiter_SetsDelimiterCorrectly()
    {
        // arrange
        SearchFilterRequest request = new("KEY", ["A", "B"]);

        // act
        request.SetFilterValuesDelimiter(",");

        // assert
        Assert.Equal(",", request.FilterValuesDelimiter);
    }

    [Fact]
    public void SetFilterValuesDelimiter_WithNull_ThrowsArgumentNullException()
    {
        // arrange
        SearchFilterRequest request = new("KEY", ["A"]);

        // act
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
            () => request.SetFilterValuesDelimiter(null!));

        // assert
        Assert.Equal("filterValuesDelimiter", ex.ParamName);
    }

    [Fact]
    public void SetFilterValuesDelimiter_WithWhitespace_ThrowsArgumentNullException()
    {
        // arrange
        SearchFilterRequest request = new("KEY", ["A"]);

        // act
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => request.SetFilterValuesDelimiter("   "));

        // assert
        Assert.Equal("filterValuesDelimiter", ex.ParamName);
    }
}
