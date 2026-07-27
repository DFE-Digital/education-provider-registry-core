using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

public sealed class StringConstantTranslationStrategyUnitTests
{
    private static StringConstantTranslationStrategy CreateStrategy() => new();
    private static StringBuilder CreateBuilder() => new();

    [Fact]
    public void CanHandle_ReturnsTrue_WhenValueIsString()
    {
        // arrange
        StringConstantTranslationStrategy strategy = CreateStrategy();

        // act
        bool result = strategy.CanHandle("Hooper");

        // assert
        Assert.True(result);
    }

    [Fact]
    public void CanHandle_ReturnsFalse_WhenValueIsNotString()
    {
        // arrange
        StringConstantTranslationStrategy strategy = CreateStrategy();

        // act
        bool resultInt = strategy.CanHandle(123);
        bool resultNull = strategy.CanHandle(null!);
        bool resultObject = strategy.CanHandle(new object());

        // assert
        Assert.False(resultInt);
        Assert.False(resultNull);
        Assert.False(resultObject);
    }

    [Fact]
    public void Write_WritesQuotedString()
    {
        // arrange
        StringConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write("Hooper", builder);

        // assert
        Assert.Equal("'Hooper'", builder.ToString());
    }

    [Fact]
    public void Write_EscapesSingleQuotes()
    {
        // arrange
        StringConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write("O'Connor", builder);

        // assert
        Assert.Equal("'O''Connor'", builder.ToString());
    }

    [Fact]
    public void Write_AllowsEmptyString()
    {
        // arrange
        StringConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(string.Empty, builder);

        // assert
        Assert.Equal("''", builder.ToString());
    }

    [Fact]
    public void Write_CanBeCalledMultipleTimes()
    {
        // arrange
        StringConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write("A", builder);
        strategy.Write("B", builder);

        // assert
        Assert.Equal("'A''B'", builder.ToString());
    }

    [Fact]
    public void Write_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        // arrange
        StringConstantTranslationStrategy strategy = CreateStrategy();

        // act/assrty
        Assert.Throws<ArgumentNullException>(() =>
            strategy.Write("Hooper", null!));
    }

    [Fact]
    public void Write_ThrowsInvalidCastException_WhenValueIsNotString()
    {
        // arrange
        StringConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act/assert
        Assert.Throws<InvalidCastException>(() =>
            strategy.Write(123, builder));
    }
}
