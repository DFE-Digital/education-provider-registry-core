using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.TestDoubles;

internal static class PropertyPathExpressionExtensions
{
    public static string GetPropertyName(this Expression expr) =>
        expr switch
        {
            MemberExpression me => me.Member.Name,
            UnaryExpression ue => GetPropertyName(ue.Operand),
            _ => throw new InvalidOperationException($"Unexpected expression type: {expr.GetType().Name}")
        };

    public static Expression GetParent(this Expression expr) =>
        expr switch
        {
            MemberExpression me => me.Expression!,
            UnaryExpression ue => GetParent(ue.Operand),
            _ => throw new InvalidOperationException($"Unexpected expression type: {expr.GetType().Name}")
        };
}
