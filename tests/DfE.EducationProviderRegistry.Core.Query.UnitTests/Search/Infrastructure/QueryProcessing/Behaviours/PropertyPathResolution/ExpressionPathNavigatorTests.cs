using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public sealed class ExpressionPathNavigatorTests
{
    [Fact]
    public void Navigate_SingleProperty_ReturnsExpectedExpression()
    {
        // arrange
        ParameterExpression root =
            Expression.Parameter(typeof(TestEntity), "rootParam");

        // act
        Expression result =
            ExpressionPathNavigator.Navigate(root, "Name");

        // assert
        Assert.Equal("Name", result.GetPropertyName());
    }

    [Fact]
    public void Navigate_NestedProperty_ReturnsExpectedExpression()
    {
        // arrange
        ParameterExpression root =
            Expression.Parameter(typeof(TestEntity), "rootParam");

        // act
        Expression result =
            ExpressionPathNavigator.Navigate(root, "Address.Postcode");

        // assert
        Assert.Equal("Postcode", result.GetPropertyName());
        Assert.Equal("Address", result.GetParent().GetPropertyName());
    }

    [Fact]
    public void Navigate_EmptyPath_ReturnsRootExpression()
    {
        // arrange
        ParameterExpression root =
            Expression.Parameter(typeof(TestEntity), "rootParam");

        // act
        Expression result =
            ExpressionPathNavigator.Navigate(root, string.Empty);

        // assert
        Assert.Same(root, result);
    }

    [Fact]
    public void Navigate_InvalidProperty_Throws()
    {
        // arrange
        ParameterExpression root =
            Expression.Parameter(typeof(TestEntity), "rootParam");

        // act / assert
        Assert.Throws<ArgumentException>(() =>
            ExpressionPathNavigator.Navigate(root, "DoesNotExist"));
    }

    [Fact]
    public void Navigate_NullRoot_Throws()
    {
        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            ExpressionPathNavigator.Navigate(null!, "Name"));
    }

    [Fact]
    public void Navigate_NullPath_Throws()
    {
        // arrange
        ParameterExpression root =
            Expression.Parameter(typeof(TestEntity), "rootParam");

        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            ExpressionPathNavigator.Navigate(root, null!));
    }
}
