using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;

public sealed class ConstantTranslationStrategyUnitTests
{
    private static ConstantTranslationStrategy CreateStrategy() => new();
    private static StringBuilder CreateBuilder() => new();

    [Fact]
    public void CanHandle_AlwaysReturnsTrue_ForAnyValue()
    {
        // arrange
        ConstantTranslationStrategy strategy = CreateStrategy();

        // act
        bool handlesInt = strategy.CanHandle(123);
        bool handlesString = strategy.CanHandle("Hooper");
        bool handlesNull = strategy.CanHandle(null!);
        bool handlesObject = strategy.CanHandle(new object());

        // assert
        Assert.True(handlesInt);
        Assert.True(handlesString);
        Assert.True(handlesNull);
        Assert.True(handlesObject);
    }

    [Fact]
    public void Write_AppendsValueToBuilder()
    {
        // arrange
        ConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(123, builder);

        // assert
        Assert.Equal("123", builder.ToString());
    }

    [Fact]
    public void Write_AppendsStringValue()
    {
        // arrange
        ConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write("Hooper", builder);

        // assert
        Assert.Equal("Hooper", builder.ToString());
    }

    [Fact]
    public void Write_AppendsNullAsEmptyString()
    {
        // arrange
        ConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(null!, builder);

        // assert
        Assert.Equal(string.Empty, builder.ToString());
    }

    [Fact]
    public void Write_AppendsObjectUsingToString()
    {
        // arrange
        ConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();
        object value = new DateTime(2026, 7, 27, 16, 45, 0);

        // act
        strategy.Write(value, builder);

        // assert
        Assert.Equal(value.ToString(), builder.ToString());
    }

    [Fact]
    public void Write_CanBeCalledMultipleTimes()
    {
        // arrange
        ConstantTranslationStrategy strategy = CreateStrategy();
        StringBuilder builder = CreateBuilder();

        // act
        strategy.Write(1, builder);
        strategy.Write(2, builder);
        strategy.Write(3, builder);

        // assert
        Assert.Equal("123", builder.ToString());
    }

    [Fact]
    public void Write_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        // arrange
        ConstantTranslationStrategy strategy = CreateStrategy();

        // act/assert
        Assert.Throws<ArgumentNullException>(() =>
            strategy.Write(123, null!));
    }
}
