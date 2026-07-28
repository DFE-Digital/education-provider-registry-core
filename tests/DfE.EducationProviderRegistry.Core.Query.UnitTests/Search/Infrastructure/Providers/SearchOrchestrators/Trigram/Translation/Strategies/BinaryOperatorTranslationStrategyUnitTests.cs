using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

public sealed class BinaryOperatorTranslationStrategyUnitTests
{
    private static BinaryOperatorTranslationStrategy CreateStrategy(string token) => new(token);

    private static StringBuilder CreateBuilder() => new();

    [Fact]
    public void Write_WritesTokenWithSpaces()
    {
        // arrange
        BinaryOperatorTranslationStrategy strategy = CreateStrategy("AND");
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(builder);

        // assert
        Assert.Equal(" AND ", builder.ToString());
    }

    [Fact]
    public void Write_WritesCorrectToken()
    {
        // arrange
        BinaryOperatorTranslationStrategy strategy = CreateStrategy("OR");
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(builder);

        // assert
        Assert.Equal(" OR ", builder.ToString());
    }

    [Fact]
    public void Write_CanBeCalledMultipleTimes()
    {
        // arrange
        BinaryOperatorTranslationStrategy strategy = CreateStrategy("=");
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(builder);
        strategy.Write(builder);

        // assert
        Assert.Equal(" =  = ", builder.ToString());
    }

    [Fact]
    public void Write_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        // arrange
        BinaryOperatorTranslationStrategy strategy = CreateStrategy("AND");

        // act/assert
        Assert.Throws<ArgumentNullException>(() =>
            strategy.Write(null!));
    }

    [Fact]
    public void Write_AllowsEmptyToken()
    {
        // arrange
        BinaryOperatorTranslationStrategy strategy = CreateStrategy(string.Empty);
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(builder);

        // assert
        Assert.Equal("  ", builder.ToString());
    }

    [Fact]
    public void Write_AllowsWhitespaceToken()
    {
        // arrange
        BinaryOperatorTranslationStrategy strategy = CreateStrategy("   ");
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(builder);

        // assert
        Assert.Equal("     ", builder.ToString());
    }

    [Fact]
    public void Write_AllowsSymbolTokens()
    {
        // arrange
        BinaryOperatorTranslationStrategy strategy = CreateStrategy("<>");
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(builder);

        // assert
        Assert.Equal(" <> ", builder.ToString());
    }
}
