using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

public sealed class PropertyEqualsSpecificationTests
{
    [Fact]
    public void ToExpression_ScalarPath_BuildsCorrectEqualityPredicate()
    {
        // arrange
        PropertyEqualsSpecification<TestEntity> spec = new("Name", "Bob");

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        BinaryExpression body = Assert.IsType<BinaryExpression>(expr.Body, exactMatch: false);

        Assert.Equal(ExpressionType.Equal, body.NodeType);
        Assert.Equal("Name", body.Left.GetPropertyName());

        ConstantExpression constant =
            Assert.IsType<ConstantExpression>(body.Right, exactMatch: false);

        Assert.Equal("Bob", constant.Value);
    }

    [Fact]
    public void ToExpression_CollectionPath_BuildsCorrectEqualityPredicate()
    {
        // arrange
        PropertyEqualsSpecification<TestEntity> spec = new("Sites[].Code", "B2");

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        MethodCallExpression call =
            Assert.IsType<MethodCallExpression>(expr.Body, exactMatch: false);

        Assert.Equal("Any", call.Method.Name);

        LambdaExpression lambda =
            Assert.IsType<LambdaExpression>(call.Arguments[1], exactMatch: false);

        BinaryExpression body =
            Assert.IsType<BinaryExpression>(lambda.Body, exactMatch: false);

        Assert.Equal(ExpressionType.Equal, body.NodeType);
        Assert.Equal("Code", body.Left.GetPropertyName());

        ConstantExpression constant =
            Assert.IsType<ConstantExpression>(body.Right, exactMatch: false);

        Assert.Equal("B2", constant.Value);
    }

    [Fact]
    public void Constructor_NullPropertyPath_Throws()
    {
        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            new PropertyEqualsSpecification<TestEntity>(null!, "Bob"));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            new PropertyEqualsSpecification<TestEntity>("Name", null!));
    }
}
