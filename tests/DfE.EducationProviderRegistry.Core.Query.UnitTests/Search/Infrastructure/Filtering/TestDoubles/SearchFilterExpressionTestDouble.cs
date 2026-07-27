using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchFilterExpressionTestDouble
{
    public static Mock<ISearchFilter<TProjection>> Mock<TProjection>()
        where TProjection : class => new(MockBehavior.Strict);

    public static Mock<ISearchFilter<DummyProjection>> MockEquals(string response)
    {
        Mock<ISearchFilter<DummyProjection>> mock = Mock<DummyProjection>();

        mock.Setup(searchFilterExpression =>
            searchFilterExpression.ToExpression(It.IsAny<SearchFilterRequest>()))
            .Returns((SearchFilterRequest req) =>
            {
                return BuildExpression(Expression.Equal, response);
            });

        return mock;
    }

    public static Mock<ISearchFilter<DummyProjection>> MockNotEquals(string response)
    {
        Mock<ISearchFilter<DummyProjection>> mock = Mock<DummyProjection>();

        mock.Setup(searchFilterExpression =>
            searchFilterExpression.ToExpression(It.IsAny<SearchFilterRequest>()))
            .Returns((SearchFilterRequest req) =>
            {
                return BuildExpression(Expression.NotEqual, response);
            });

        return mock;
    }

    public static Mock<ISearchFilter<TProjection>> MockForExpression<TProjection>(
        Expression<Func<TProjection, bool>> expressionTree)
    where TProjection : class
    {
        Mock<ISearchFilter<TProjection>> exprMock = Mock<TProjection>();

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
