using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared;

public sealed class GovernanceIdentifierTests
{
    [Fact]
    public void Constructor_WithValidSevenDigitValue_CreatesInstance()
    {
        string input = "1234567";
        GovernanceIdentifier identifier = new(input);

        Assert.Equal(input, identifier.Value);
    }

    [Fact]
    public void Constructor_WithNonDigitCharacters_ThrowsArgumentException()
    {
        string input = "A123456";

        Assert.Throws<ArgumentException>(() => new GovernanceIdentifier(input));
    }

    [Fact]
    public void Constructor_WithTooShortValue_ThrowsArgumentException()
    {
        string input = "123456";

        Assert.Throws<ArgumentException>(() => new GovernanceIdentifier(input));
    }

    [Fact]
    public void Constructor_WithTooLongValue_ThrowsArgumentException()
    {
        string input = "12345678";

        Assert.Throws<ArgumentException>(() => new GovernanceIdentifier(input));
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        string input = "1234567";
        GovernanceIdentifier identifier = new(input);

        string result = identifier.ToString();

        Assert.Equal(input, result);
    }
}
