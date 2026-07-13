using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Pipeline;

public sealed class SearchPipelineContextUnitTests
{
    [Fact]
    public void Set_StoresValue_ByConcreteType()
    {
        // arrange
        SearchPipelineContext context = new();

        // act
        context.Set<int>(42);

        // assert
        int result = context.Get<int>();
        Assert.Equal(42, result);
    }

    [Fact]
    public void Get_ReturnsStoredValue_WhenTypeMatches()
    {
        // arrange
        SearchPipelineContext context = new();
        context.Set<string>("hello");

        // act
        string value = context.Get<string>();

        // assert
        Assert.Equal("hello", value);
    }

    [Fact]
    public void Get_ThrowsInvalidOperationException_WhenTypeNotStored()
    {
        // arrange
        SearchPipelineContext context = new();

        // act + assert
        InvalidOperationException ex =
            Assert.Throws<InvalidOperationException>(() => context.Get<string>());

        Assert.Contains("PipelineContext does not contain a value of type String", ex.Message);
    }

    [Fact]
    public void TryGet_ReturnsTrue_AndOutputsValue_WhenTypeStored()
    {
        // arrange
        SearchPipelineContext context = new();
        context.Set<bool>(true);

        // act
        bool success = context.TryGet<bool>(out bool value);

        // assert
        Assert.True(success);
        Assert.True(value);
    }

    [Fact]
    public void TryGet_ReturnsFalse_AndOutputsDefault_WhenTypeNotStored()
    {
        // arrange
        SearchPipelineContext context = new();

        // act
        bool success = context.TryGet<int>(out int value);

        // assert
        Assert.False(success);
        Assert.Equal(default, value);
    }

    [Fact]
    public void Set_OverwritesExistingValue_ForSameType()
    {
        // arrange
        SearchPipelineContext context = new();
        context.Set<int>(1);

        // act
        context.Set<int>(99);

        // assert
        int result = context.Get<int>();
        Assert.Equal(99, result);
    }

    [Fact]
    public void Set_AllowsDifferentTypes_ToCoexist()
    {
        // arrange
        SearchPipelineContext context = new();
        context.Set<int>(123);
        context.Set<string>("abc");

        // act
        int intValue = context.Get<int>();
        string stringValue = context.Get<string>();

        // assert
        Assert.Equal(123, intValue);
        Assert.Equal("abc", stringValue);
    }

    [Fact]
    public void Get_Throws_WhenRequestingInterfaceInsteadOfConcreteType()
    {
        // arrange
        SearchPipelineContext context = new();
        List<string> list = ["x", "y"];
        context.Set(list);

        // act/assert
        Assert.Throws<InvalidOperationException>(() =>
            context.Get<IReadOnlyCollection<string>>());
    }
}
