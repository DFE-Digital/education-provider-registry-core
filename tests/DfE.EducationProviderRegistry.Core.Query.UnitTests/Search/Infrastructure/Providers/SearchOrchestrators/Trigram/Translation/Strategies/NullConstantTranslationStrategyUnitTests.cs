using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

public sealed class NullConstantTranslationStrategyUnitTests
{
    private static NullConstantTranslationStrategy CreateStrategy() => new();
    private static StringBuilder CreateBuilder() => new();


    [Fact]
    public void CanHandle_ReturnsTrue_WhenValueIsNull()
    {
        // arrange
        NullConstantTranslationStrategy strategy = CreateStrategy();

        // act
        bool result = strategy.CanHandle(null!);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void CanHandle_ReturnsFalse_WhenValueIsNotNull()
    {
        // arrange
        NullConstantTranslationStrategy strategy = CreateStrategy();

        // act
        bool resultInt = strategy.CanHandle(123);
        bool resultString = strategy.CanHandle("Hooper");
        bool resultObject = strategy.CanHandle(new object());

        // assert
        Assert.False(resultInt);
        Assert.False(resultString);
        Assert.False(resultObject);
    }

    [Fact]
    public void Write_AppendsNullLiteral()
    {
        // arrange
        NullConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(null!, builder);

        // assert
        Assert.Equal("NULL", builder.ToString());
    }

    [Fact]
    public void Write_IgnoresProvidedValue()
    {
        // arrange
        NullConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write("Hooper", builder);

        // assert
        Assert.Equal("NULL", builder.ToString());
    }

    [Fact]
    public void Write_CanBeCalledMultipleTimes()
    {
        // arrange
        NullConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(null!, builder);
        strategy.Write(null!, builder);

        // assert
        Assert.Equal("NULLNULL", builder.ToString());
    }

    [Fact]
    public void Write_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        // arrange
        NullConstantTranslationStrategy strategy = CreateStrategy();

        // act/assert
        Assert.Throws<ArgumentNullException>(() =>
            strategy.Write(null!, null!));
    }
}
