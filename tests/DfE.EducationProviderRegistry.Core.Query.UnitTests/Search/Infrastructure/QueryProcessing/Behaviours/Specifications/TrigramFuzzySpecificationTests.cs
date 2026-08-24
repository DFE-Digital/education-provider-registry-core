using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

public sealed class TrigramFuzzySpecificationTests
{
    [Fact]
    public void ToExpression_ScalarPath_BuildsCorrectSimilarityCall()
    {
        // arrange
        TrigramFuzzySpecification<TestEntity> spec = new("Name", "Bob", 0.5);

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        BinaryExpression body = Assert.IsType<BinaryExpression>(expr.Body, exactMatch: false);
        Assert.Equal(ExpressionType.GreaterThanOrEqual, body.NodeType);
        MethodCallExpression call = Assert.IsType<MethodCallExpression>(body.Left, exactMatch: false);
        Assert.Equal("TrigramsWordSimilarity", call.Method.Name);
        Assert.Equal("Bob", ((ConstantExpression)call.Arguments[1]).Value);
        Assert.Equal("Name", call.Arguments[2].GetPropertyName());
    }

    [Fact]
    public void ToExpression_NestedScalarPath_BuildsCorrectSimilarityCall()
    {
        // arrange
        TrigramFuzzySpecification<TestEntity> spec = new("Address.Postcode", "DL1", 0.7);

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        BinaryExpression body = Assert.IsType<BinaryExpression>(expr.Body, exactMatch: false);
        MethodCallExpression call = Assert.IsType<MethodCallExpression>(body.Left, exactMatch: false);
        Assert.Equal("TrigramsWordSimilarity", call.Method.Name);
        Assert.Equal("DL1", ((ConstantExpression)call.Arguments[1]).Value);
        Assert.Equal("Postcode", call.Arguments[2].GetPropertyName());
        Assert.Equal("Address", call.Arguments[2].GetParent().GetPropertyName());
    }

    [Fact]
    public void ToExpression_CollectionPath_BuildsAnyCall()
    {
        // arrange
        TrigramFuzzySpecification<TestEntity> spec = new("Sites[].Code", "X1", 0.3);

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        MethodCallExpression anyCall = Assert.IsType<MethodCallExpression>(expr.Body, exactMatch: false);
        Assert.Equal("Any", anyCall.Method.Name);
        LambdaExpression lambda = Assert.IsType<LambdaExpression>(anyCall.Arguments[1], exactMatch: false);
        BinaryExpression body = Assert.IsType<BinaryExpression>(lambda.Body, exactMatch: false);
        MethodCallExpression trigramCall = Assert.IsType<MethodCallExpression>(body.Left, exactMatch: false);
        Assert.Equal("TrigramsWordSimilarity", trigramCall.Method.Name);
        Assert.Equal("X1", ((ConstantExpression)trigramCall.Arguments[1]).Value);
        Assert.Equal("Code", trigramCall.Arguments[2].GetPropertyName());
    }

    [Fact]
    public void ToExpression_CollectionNestedPath_BuildsCorrectElementSimilarityCall()
    {
        // arrange
        TrigramFuzzySpecification<TestEntity> spec = new("Sites[].Location.Town", "York", 0.4);

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        MethodCallExpression anyCall = Assert.IsType<MethodCallExpression>(expr.Body, exactMatch: false);
        LambdaExpression lambda = Assert.IsType<LambdaExpression>(anyCall.Arguments[1], exactMatch: false);
        BinaryExpression body = Assert.IsType<BinaryExpression>(lambda.Body, exactMatch: false);
        MethodCallExpression trigramCall = Assert.IsType<MethodCallExpression>(body.Left, exactMatch: false);
        Assert.Equal("TrigramsWordSimilarity", trigramCall.Method.Name);
        Assert.Equal("York", ((ConstantExpression)trigramCall.Arguments[1]).Value);
        Assert.Equal("Town", trigramCall.Arguments[2].GetPropertyName());
        Assert.Equal("Location", trigramCall.Arguments[2].GetParent().GetPropertyName());
    }

    [Fact]
    public void ToExpression_ScalarPath_ContainsCorrectThresholdComparison()
    {
        // arrange
        TrigramFuzzySpecification<TestEntity> spec = new("Name", "Bob", 0.5);

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        BinaryExpression body = Assert.IsType<BinaryExpression>(expr.Body, exactMatch: false);
        Assert.Equal(ExpressionType.GreaterThanOrEqual, body.NodeType);
        ConstantExpression threshold = Assert.IsType<ConstantExpression>(body.Right);
        Assert.Equal(0.5, threshold.Value);
    }

    [Fact]
    public void ToExpression_ScalarPath_ContainsCorrectTrigramCall()
    {
        // arrange
        TrigramFuzzySpecification<TestEntity> spec = new("Name", "Bob", 0.5);

        // act
        Expression<Func<TestEntity, bool>> expr = spec.ToExpression();

        // assert
        BinaryExpression body = Assert.IsType<BinaryExpression>(expr.Body, exactMatch: false);
        MethodCallExpression call = Assert.IsType<MethodCallExpression>(body.Left, exactMatch: false);
        Assert.Equal("TrigramsWordSimilarity", call.Method.Name);
        Assert.Equal("Bob", ((ConstantExpression)call.Arguments[1]).Value);
        Assert.Equal("Name", call.Arguments[2].GetPropertyName());
    }
}
