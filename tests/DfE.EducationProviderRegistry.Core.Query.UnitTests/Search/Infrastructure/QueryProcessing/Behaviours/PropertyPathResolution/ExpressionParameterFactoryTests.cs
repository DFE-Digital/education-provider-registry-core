using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public sealed class ExpressionParameterFactoryTests
{
    [Fact]
    public void CreateRootParameter_ReturnsExpectedParameter()
    {
        // act
        ParameterExpression result =
            ExpressionParameterFactory.CreateRootParameter<TestEntity>();

        // assert
        Assert.Equal(typeof(TestEntity), result.Type);
        Assert.Equal("rootParam", result.Name);
    }

    [Fact]
    public void CreateElementParameter_ReturnsExpectedParameter()
    {
        // arrange
        Type elementType = typeof(string);

        // act
        ParameterExpression result =
            ExpressionParameterFactory.CreateElementParameter(elementType);

        // assert
        Assert.Equal(elementType, result.Type);
        Assert.Equal("elementParam", result.Name);
    }

    [Fact]
    public void CreateElementParameter_NullElementType_Throws()
    {
        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            ExpressionParameterFactory.CreateElementParameter(null!));
    }
}
