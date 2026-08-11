using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public sealed class ParsedPropertyPathTests
{
    [Fact]
    public void Constructor_SetsExpectedProperties()
    {
        // act
        ParsedPropertyPath result = new(
            true,
            "Sites",
            "Location.Town");

        // assert
        Assert.True(result.IsCollection);
        Assert.Equal("Sites", result.NavigationName);
        Assert.Equal("Location.Town", result.RemainderPath);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // arrange
        ParsedPropertyPath first = new(
            true,
            "Sites",
            "Location.Town");

        ParsedPropertyPath second = new(
            true,
            "Sites",
            "Location.Town");

        // act / assert
        Assert.Equal(first, second);
    }
}
