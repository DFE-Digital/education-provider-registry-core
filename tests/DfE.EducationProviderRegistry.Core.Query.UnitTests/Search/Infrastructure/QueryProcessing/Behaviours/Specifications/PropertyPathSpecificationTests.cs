using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

public sealed class PropertyPathSpecificationTests
{
    [Fact]
    public void ToExpression_ScalarPath_BuildsCorrectPredicate()
    {
        // arrange
        TestDoubles.SpecificationStub<TestEntity> spec = new("Name", "Bob");

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        Assert.NotNull(expr);
        Assert.Equal("Name", ((BinaryExpression)expr.Body).Left.GetPropertyName());
    }

    [Fact]
    public void ToExpression_NestedScalarPath_BuildsCorrectPredicate()
    {
        // arrange
        TestDoubles.SpecificationStub<TestEntity> spec = new("Address.Postcode", "DL1");

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        BinaryExpression body = (BinaryExpression)expr.Body;
        Assert.Equal("Postcode", body.Left.GetPropertyName());
        Assert.Equal("Address", body.Left.GetParent().GetPropertyName());
    }

    [Fact]
    public void ToExpression_CollectionPath_BuildsAnyPredicate()
    {
        // arrange
        TestDoubles.SpecificationStub<TestEntity> spec = new("Sites[].Code", "X1");

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        Assert.NotNull(expr);

        MethodCallExpression call = Assert.IsType<MethodCallExpression>(expr.Body, exactMatch: false);
        Assert.Equal("Any", call.Method.Name);

        LambdaExpression lambda = Assert.IsType<LambdaExpression>(call.Arguments[1], exactMatch: false);
        BinaryExpression body = Assert.IsType<BinaryExpression>(lambda.Body, exactMatch: false);

        Assert.Equal("Code", body.Left.GetPropertyName());
    }

    [Fact]
    public void ToExpression_CollectionNestedPath_BuildsCorrectElementPredicate()
    {
        // arrange
        TestDoubles.SpecificationStub<TestEntity> spec = new("Sites[].Location.Town", "York");

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        MethodCallExpression call = Assert.IsType<MethodCallExpression>(expr.Body, exactMatch: false);
        LambdaExpression lambda = Assert.IsType<LambdaExpression>(call.Arguments[1], exactMatch: false);
        BinaryExpression body = Assert.IsType<BinaryExpression>(lambda.Body, exactMatch: false);

        Assert.Equal("Town", body.Left.GetPropertyName());
        Assert.Equal("Location", body.Left.GetParent().GetPropertyName());
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenPredicateMatches()
    {
        // arrange
        TestEntity entity = new() { Name = "Bob" };
        TestDoubles.SpecificationStub<TestEntity> spec = new("Name", "Bob");

        // act
        bool result = spec.IsSatisfiedBy(entity);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenPredicateDoesNotMatch()
    {
        // arrange
        TestEntity entity = new() { Name = "Alice" };
        TestDoubles.SpecificationStub<TestEntity> spec = new("Name", "Bob");

        // act
        bool result = spec.IsSatisfiedBy(entity);

        // assert
        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_CollectionPredicate_WorksCorrectly()
    {
        // arrange
        TestEntity entity = new()
        {
            Sites =
            [
                new Site { Code = "A1" },
                new Site { Code = "B2" }
            ]
        };

        TestDoubles.SpecificationStub<TestEntity> spec = new("Sites[].Code", "B2");

        // act
        bool result = spec.IsSatisfiedBy(entity);

        // assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_CollectionPredicate_ReturnsFalse_WhenNoMatch()
    {
        // arrange
        TestEntity entity = new()
        {
            Sites =
            [
                new Site { Code = "A1" },
                new Site { Code = "B2" }
            ]
        };

        TestDoubles.SpecificationStub<TestEntity> spec = new("Sites[].Code", "ZZZ");

        // act
        bool result = spec.IsSatisfiedBy(entity);

        // assert
        Assert.False(result);
    }
}
