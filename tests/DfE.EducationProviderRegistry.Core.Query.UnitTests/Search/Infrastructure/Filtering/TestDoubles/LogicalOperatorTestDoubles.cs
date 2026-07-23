using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.LogicalOperators;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class LogicalOperatorTestDoubles
{
    public static Mock<ILogicalOperator<TProjection>> Mock<TProjection>()
        where TProjection : class
    {
        return new Mock<ILogicalOperator<TProjection>>(MockBehavior.Strict);
    }

    public static Mock<ILogicalOperator<TProjection>> MockFor<TProjection>(
        Func<
            Expression<Func<TProjection, bool>>,
            Expression<Func<TProjection, bool>>,
            Expression<Func<TProjection, bool>>> combine)
        where TProjection : class
    {
        Mock<ILogicalOperator<TProjection>> opMock = Mock<TProjection>();
            
        opMock
            .Setup(logicalOperator =>
                logicalOperator.Combine(
                    It.IsAny<Expression<Func<TProjection, bool>>>(),
                    It.IsAny<Expression<Func<TProjection, bool>>>()))
                .Returns((Expression<Func<TProjection, bool>> left,
                          Expression<Func<TProjection, bool>> right) =>
                {
                    return combine(left, right);
                });

        return opMock;
    }

    private static Expression<Func<TProjection, bool>> BuildBinary<TProjection>(
        Expression<Func<TProjection, bool>> left,
        Expression<Func<TProjection, bool>> right,
        Func<Expression, Expression, BinaryExpression> op)
        where TProjection : class
    {
        ParameterExpression param =
            Expression.Parameter(typeof(TProjection), "dummy");

        Expression leftBody = Expression.Invoke(left, param);
        Expression rightBody = Expression.Invoke(right, param);

        BinaryExpression body = op(leftBody, rightBody);

        Expression<Func<TProjection, bool>> expr =
            Expression.Lambda<Func<TProjection, bool>>(body, param);

        return expr;
    }

    public static Mock<ILogicalOperator<TProjection>> MockAnd<TProjection>()
        where TProjection : class
    {
        return MockFor<TProjection>(
            (Expression<Func<TProjection, bool>> left,
             Expression<Func<TProjection, bool>> right) =>
            {
                return BuildBinary(left, right, Expression.AndAlso);
            });
    }

    public static Mock<ILogicalOperator<TProjection>> MockOr<TProjection>()
        where TProjection : class
    {
        return MockFor<TProjection>(
            (Expression<Func<TProjection, bool>> left,
             Expression<Func<TProjection, bool>> right) =>
            {
                return BuildBinary(left, right, Expression.OrElse);
            });
    }
}
