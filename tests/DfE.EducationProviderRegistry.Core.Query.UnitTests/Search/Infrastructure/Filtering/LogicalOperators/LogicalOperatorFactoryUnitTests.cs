using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators.Factories;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.LogicalOperators;

public sealed class LogicalOperatorFactoryUnitTests
{
    private LogicalOperatorFactory<DummyProjection> CreateFactory()
    {
        Dictionary<string, Func<ILogicalOperator<DummyProjection>>> registry =
            new()
            {
                { "AndLogicalOperator", () =>
                    LogicalOperatorTestDoubles.MockAnd<DummyProjection>().Object },
                { "OrLogicalOperator", () =>
                    LogicalOperatorTestDoubles.MockOr<DummyProjection>().Object }
            };

        return new LogicalOperatorFactory<DummyProjection>(registry);
    }

    [Fact]
    public void Resolve_ValidName_ReturnsCorrectInstance_A()
    {
        // arrange
        LogicalOperatorFactory<DummyProjection> factory = CreateFactory();

        // act
        ILogicalOperator<DummyProjection> result =
            factory.Resolve("AndLogicalOperator");

        // assert
        Assert.IsType<ILogicalOperator<DummyProjection>>(result, exactMatch: false);
    }

    [Fact]
    public void Resolve_ValidName_ReturnsCorrectInstance_B()
    {
        // arrange
        LogicalOperatorFactory<DummyProjection> factory = CreateFactory();

        // act
        ILogicalOperator<DummyProjection> result =
            factory.Resolve("OrLogicalOperator");

        // assert
        Assert.IsType<ILogicalOperator<DummyProjection>>(result, exactMatch: false);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Resolve_InvalidName_ThrowsArgumentException(string name)
    {
        // arrange
        LogicalOperatorFactory<DummyProjection> factory = CreateFactory();

        // act/assert
        Assert.Throws<ArgumentException>(() =>
            factory.Resolve(name));
    }

    [Fact]
    public void Resolve_UnknownName_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        LogicalOperatorFactory<DummyProjection> factory = CreateFactory();

        // act/assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            factory.Resolve("DoesNotExist"));
    }

    [Fact]
    public void Resolve_DelegateIsInvoked()
    {
        // arrange
        bool invoked = false;

        Dictionary<string, Func<ILogicalOperator<DummyProjection>>> registry =
            new()
            {
                {
                    "TestOperator",
                    () =>
                    {
                        invoked = true;
                        return LogicalOperatorTestDoubles.MockAnd<DummyProjection>().Object;
                    }
                }
            };

        LogicalOperatorFactory<DummyProjection> factory = new(registry);

        // act
        ILogicalOperator<DummyProjection> result = factory.Resolve("TestOperator");

        // assert
        Assert.True(invoked);
        Assert.IsType<ILogicalOperator<DummyProjection>>(result, exactMatch: false);
    }
}
