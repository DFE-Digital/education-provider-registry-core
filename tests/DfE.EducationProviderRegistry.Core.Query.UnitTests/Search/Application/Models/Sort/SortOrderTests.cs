using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Sort;

public class SortOrderTests
{
    [Fact]
    public void Constructor_WithValidFieldAndDirection_ShouldInitializeCorrectly()
    {
        // arrange
        List<string> validFields = ["Surname", "DOB", "Forename"];
        string field = "Surname";
        string direction = "desc";

        // act
        SortOrder sortOrder = new(field, direction, validFields);

        // Assert
        Assert.Equal("Surname desc", sortOrder.Value);
        Assert.Equal("Surname desc", sortOrder.ToString());
    }

    [Fact]
    public void Constructor_WithInvalidField_ShouldThrowArgumentException()
    {
        // arrange
        List<string> validFields = ["Surname", "DOB"];

        // act
        ArgumentException exception =
            Assert.Throws<ArgumentException>(() =>
                new SortOrder("Age", "asc", validFields));

        // Assert
        Assert.Contains("Unknown sort field 'Age'", exception.Message);
        Assert.Equal("sortField", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidDirection_ShouldThrowArgumentException()
    {
        // arrange
        List<string> validFields = ["Surname", "DOB"];

        // act
        ArgumentException exception =
            Assert.Throws<ArgumentException>(() =>
                new SortOrder("DOB", "upward", validFields));

        // Assert
        Assert.Contains("Unknown sort direction 'upward'", exception.Message);
        Assert.Equal("direction", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldReturnValidSortOrderInstance()
    {
        // arrange
        List<string> validFields = ["Level", "Subject"];

        // act
        SortOrder sortOrder =
            SortOrder.Create("Subject", "ASC", validFields);

        // Assert
        Assert.Equal("Subject asc", sortOrder.Value);
    }

    [Fact]
    public void ToString_ShouldReturnSameValueAsSortExpression()
    {
        // arrange
        List<string> validFields = ["Region"];
        SortOrder sortOrder = new("Region", "DESC", validFields);

        // act & Assert
        Assert.Equal("Region desc", sortOrder.ToString());
    }
}
