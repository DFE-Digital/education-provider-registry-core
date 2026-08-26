using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Sort;

public sealed class SortFieldTests
{
    [Fact]
    public void Constructor_WithValidField_ShouldInitializeCorrectly()
    {
        // arrange
        List<string> validFields = ["Surname", "DOB", "Forename"];
        string inputField = "DOB";

        // act
        SortField sortField = new(inputField, validFields);

        // Assert
        Assert.Equal(inputField, sortField.Value);
        Assert.Equal(validFields.Count, sortField.ValidFields.Count);

        bool allMatch = validFields
            .Zip(sortField.ValidFields, (expected, actual) => expected == actual)
            .All(match => match);

        Assert.True(allMatch);

        Assert.True(sortField.IsValid("dob")); // Case-insensitive check
    }

    [Fact]
    public void Constructor_WithNullField_ShouldThrowArgumentNullException()
    {
        // arrange
        List<string> validFields = ["Surname", "DOB"];

        // act
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() =>
                new SortField(null!, validFields));

        // Assert
        Assert.Equal("sortField", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithEmptyValidFields_ShouldThrowArgumentException()
    {
        // act
        ArgumentException exception =
            Assert.Throws<ArgumentException>(() =>
                new SortField("Surname", new List<string>()));

        // Assert
        Assert.Contains("cannot be null or empty", exception.Message);
        Assert.Equal("validSortFields", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithDuplicateValidFields_ShouldThrowArgumentException()
    {
        // arrange
        List<string> validFields = ["DOB", "dob"];

        // act
        ArgumentException exception =
            Assert.Throws<ArgumentException>(() =>
                new SortField("DOB", validFields));

        // Assert
        Assert.Contains("contains duplicate entries", exception.Message);
        Assert.Equal("validSortFields", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidField_ShouldThrowArgumentException()
    {
        // arrange
        List<string> validFields = ["Surname", "DOB"];

        // act
        ArgumentException exception =
            Assert.Throws<ArgumentException>(() =>
                new SortField("Age", validFields));

        // Assert
        Assert.Contains("Unknown sort field 'Age'", exception.Message);
        Assert.Equal("sortField", exception.ParamName);
    }

    [Fact]
    public void IsValid_ShouldReturnTrueForCaseInsensitiveMatch()
    {
        // arrange
        List<string> validFields = ["Surname", "DOB"];
        SortField sortField = new("DOB", validFields);

        // act & Assert
        Assert.True(sortField.IsValid("dob"));
        Assert.True(sortField.IsValid("SURNAME"));
        Assert.False(sortField.IsValid("Forename"));
    }

    [Fact]
    public void Create_ShouldReturnValidSortFieldInstance()
    {
        // arrange
        List<string> validFields = ["Level", "Subject"];

        // act
        SortField sortField =
            SortField.Create("Level", validFields);

        // Assert
        Assert.Equal("Level", sortField.Value);

        Assert.Equal(validFields.Count, sortField.ValidFields.Count);

        bool allMatch = validFields
            .Zip(sortField.ValidFields, (expected, actual) => expected == actual)
            .All(match => match);

        Assert.True(allMatch);
    }
}
