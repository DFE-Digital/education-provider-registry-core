using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

public sealed class LikeSpecificationTests
{
    [Fact]
    public void ToExpression_ScalarPath_BuildsExpectedILikeExpression()
    {
        // arrange
        LikeSpecification<TestEntity> spec = new("Name", "school");

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        MethodCallExpression call =
            Assert.IsType<MethodCallExpression>(expr.Body, exactMatch: false);

        Assert.Equal("ILike", call.Method.Name);

        ConstantExpression pattern =
            Assert.IsType<ConstantExpression>(call.Arguments[2], exactMatch: false);

        Assert.Equal("%school%", pattern.Value);
    }

    [Fact]
    public void ToExpression_CollectionPath_BuildsExpectedILikeExpression()
    {
        // arrange
        LikeSpecification<TestEntity> spec = new("Sites[].Code", "abc");

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        MethodCallExpression call =
            Assert.IsType<MethodCallExpression>(expr.Body, exactMatch: false);

        Assert.Equal("Any", call.Method.Name);

        LambdaExpression lambda =
            Assert.IsType<LambdaExpression>(call.Arguments[1], exactMatch: false);

        MethodCallExpression ilikeCall =
            Assert.IsType<MethodCallExpression>(lambda.Body, exactMatch: false);

        Assert.Equal("ILike", ilikeCall.Method.Name);

        ConstantExpression pattern =
            Assert.IsType<ConstantExpression>(ilikeCall.Arguments[2], exactMatch: false);

        Assert.Equal("%abc%", pattern.Value);
    }

    [Fact]
    public void Constructor_NullPropertyPath_Throws()
    {
        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            new LikeSpecification<TestEntity>(null!, "school"));
    }

    [Fact]
    public void Constructor_NullValue_Throws()
    {
        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            new LikeSpecification<TestEntity>("Name", null!));
    }
}
