using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Extensions;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.LogicalOperators;

public sealed class StringExtensionsUnitTests
{
    [Fact]
    public void PadSides_DefaultPaddingWidth_AppliesOneSpaceEachSide()
    {
        // arrange
        string value = "ABC";

        // act
        string result = value.PadSides();

        // assert
        Assert.Equal(" ABC ", result);
    }

    [Fact]
    public void PadSides_CustomPaddingWidth_AppliesCorrectPadding()
    {
        // arrange
        string value = "ABC";

        // act
        string result = value.PadSides(2);

        // assert
        Assert.Equal("  ABC  ", result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void PadSides_InvalidString_ThrowsArgumentNullException(string value)
    {
        Assert.Throws<ArgumentException>(() => value!.PadSides());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void PadSides_InvalidPaddingWidth_ThrowsArgumentException(int paddingWidth)
    {
        // arrange
        string value = "ABC";

        // assert
        Assert.Throws<ArgumentException>(() => value.PadSides(paddingWidth));
    }

    [Fact]
    public void PadSides_SingleCharacterString_PadsCorrectly()
    {
        // arrange
        string value = "X";

        // act
        string result = value.PadSides(1);

        // assert
        Assert.Equal(" X ", result);
    }

    [Fact]
    public void PadSides_LongString_PadsCorrectly()
    {
        // arrange
        string value = "LongString";

        // act
        string result = value.PadSides(3);

        // assert
        int expectedLeft = value.Length + 3;
        int expectedRight = expectedLeft + 3;

        Assert.Equal(value.PadLeft(expectedLeft).PadRight(expectedRight), result);
    }
}
