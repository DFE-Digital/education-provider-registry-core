using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Filter;

public sealed class FilterRequestTests
{
    [Fact]
    public void Constructor_WithValidArguments_ShouldInitializeProperties()
    {
        // arrange
        string filterName = "Region";
        List<object> filterValues = ["North", "South"];

        // act
        FilterRequest request = new(filterName, filterValues);

        // assert
        Assert.Equal(filterName, request.FilterName);
        Assert.NotNull(request.FilterValues);
        Assert.Equal(filterValues.Count, request.FilterValues.Count);

        Assert.True(request.FilterValues.CollectionsMatch(filterValues));
    }

    [Fact]
    public void Constructor_WithNullFilterName_ShouldThrowArgumentNullException()
    {
        // arrange
        IList<object> values = ["Value1"];

        // act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() =>
                new FilterRequest(null!, values));

        // assert
        Assert.Equal("filterName", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullFilterValues_ShouldThrowArgumentNullException()
    {
        // act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() =>
                new FilterRequest("Field", null!));

        // assert
        Assert.Equal("filterValues", exception.ParamName);
    }

    [Fact]
    public void FilterValues_ShouldBeReadOnly()
    {
        // arrange
        List<object> values = ["A", "B"];
        FilterRequest request = new("Field", values);

        // act
        IList<object> readOnlyValues = request.FilterValues;

        // assert
        Assert.True(readOnlyValues.IsReadOnly);

        Assert.Throws<InvalidCastException>(() =>
        {
            List<object> list = (List<object>)readOnlyValues;
            list.Add("C");
        });
    }
}
