using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public sealed class ResolvedPathTests
{
    [Fact]
    public void Constructor_SetsExpectedProperties()
    {
        // arrange
        ParameterExpression rootParameter =
            Expression.Parameter(typeof(string), "rootParam");

        Expression accessExpression =
            Expression.Property(
                Expression.Constant("test"),
                nameof(string.Length));

        ParameterExpression collectionElementParameter =
            Expression.Parameter(typeof(int), "elementParam");

        // act
        ResolvedPath result = new(
            rootParameter,
            accessExpression,
            true,
            collectionElementParameter,
            "Items");

        // assert
        Assert.Same(rootParameter, result.RootParameter);
        Assert.Same(accessExpression, result.AccessExpression);
        Assert.True(result.IsCollection);
        Assert.Same(collectionElementParameter, result.CollectionElementParameter);
        Assert.Equal("Items", result.CollectionNavigationName);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // arrange
        ParameterExpression rootParameter =
            Expression.Parameter(typeof(string), "rootParam");

        Expression accessExpression =
            Expression.Property(
                Expression.Constant("test"),
                nameof(string.Length));

        ParameterExpression collectionElementParameter =
            Expression.Parameter(typeof(int), "elementParam");

        ResolvedPath first = new(
            rootParameter,
            accessExpression,
            true,
            collectionElementParameter,
            "Items");

        ResolvedPath second = new(
            rootParameter,
            accessExpression,
            true,
            collectionElementParameter,
            "Items");

        // act / assert
        Assert.Equal(first, second);
    }
}
