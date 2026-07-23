using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFilterExpressionTestDouble
{
    public static Mock<ISearchFilterExpression<TProjection>> Mock<TProjection>()
        where TProjection : class => new(MockBehavior.Strict);

    public static Mock<ISearchFilterExpression<DummyProjection>> MockEquals(string response)
    {
        Mock<ISearchFilterExpression<DummyProjection>> mock = Mock<DummyProjection>();

        mock.Setup(searchFilterExpression =>
            searchFilterExpression.ToExpression(It.IsAny<SearchFilterRequest>()))
            .Returns((SearchFilterRequest req) =>
            {
                return BuildExpression(Expression.Equal, response);
            });

        return mock;
    }

    public static Mock<ISearchFilterExpression<DummyProjection>> MockNotEquals(string response)
    {
        Mock<ISearchFilterExpression<DummyProjection>> mock = Mock<DummyProjection>();

        mock.Setup(searchFilterExpression =>
            searchFilterExpression.ToExpression(It.IsAny<SearchFilterRequest>()))
            .Returns((SearchFilterRequest req) =>
            {
                return BuildExpression(Expression.NotEqual, response);
            });

        return mock;
    }

    public static Mock<ISearchFilterExpression<TProjection>> MockForExpression<TProjection>(
        Expression<Func<TProjection, bool>> expressionTree)
    where TProjection : class
    {
        Mock<ISearchFilterExpression<TProjection>> exprMock = Mock<TProjection>();

        exprMock
            .Setup(searchFilterExpression =>
                searchFilterExpression.ToExpression(It.IsAny<SearchFilterRequest>()))
            .Returns(expressionTree)
            .Verifiable();

        return exprMock;
    }

    private static Expression<Func<DummyProjection, bool>> BuildExpression(
        Func<Expression, Expression, BinaryExpression> comparison,
        string response)
    {
        ParameterExpression param = Expression.Parameter(typeof(DummyProjection), "dummy");
        MemberExpression property = Expression.Property(param, nameof(DummyProjection.Value));
        ConstantExpression constant = Expression.Constant(response);
        BinaryExpression body = comparison(property, constant);

        Expression<Func<DummyProjection, bool>> expr =
            Expression.Lambda<Func<DummyProjection, bool>>(body, param);

        return expr;
    }
}
