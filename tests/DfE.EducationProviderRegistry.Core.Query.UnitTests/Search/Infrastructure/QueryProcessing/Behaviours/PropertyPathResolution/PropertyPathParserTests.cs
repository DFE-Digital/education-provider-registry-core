using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public sealed class PropertyPathParserTests
{
    [Fact]
    public void Parse_ScalarPath_ReturnsExpectedResult()
    {
        // arrange
        string fieldPath = "Address.Postcode";

        // act
        ParsedPropertyPath result = PropertyPathParser.Parse(fieldPath);

        // assert
        Assert.False(result.IsCollection);
        Assert.Equal("Address.Postcode", result.NavigationName);
        Assert.Equal(string.Empty, result.RemainderPath);
    }

    [Fact]
    public void Parse_CollectionPath_ReturnsExpectedResult()
    {
        // arrange
        string fieldPath = "Sites[].Location.Town";

        // act
        ParsedPropertyPath result = PropertyPathParser.Parse(fieldPath);

        // assert
        Assert.True(result.IsCollection);
        Assert.Equal("Sites", result.NavigationName);
        Assert.Equal("Location.Town", result.RemainderPath);
    }

    [Fact]
    public void Parse_CollectionPathWithNoRemainder_ReturnsEmptyRemainder()
    {
        // arrange
        string fieldPath = "Sites[]";

        // act
        ParsedPropertyPath result = PropertyPathParser.Parse(fieldPath);

        // assert
        Assert.True(result.IsCollection);
        Assert.Equal("Sites", result.NavigationName);
        Assert.Equal(string.Empty, result.RemainderPath);
    }

    [Fact]
    public void Parse_NullFieldPath_Throws()
    {
        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            PropertyPathParser.Parse(null!));
    }
}
