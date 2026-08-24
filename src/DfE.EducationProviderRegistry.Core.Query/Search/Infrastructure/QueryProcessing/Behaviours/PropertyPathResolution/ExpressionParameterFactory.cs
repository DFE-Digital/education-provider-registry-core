using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public static class ExpressionParameterFactory
{
    public static ParameterExpression CreateRootParameter<TEntity>() =>
        Expression.Parameter(typeof(TEntity), "rootParam");

    public static ParameterExpression CreateElementParameter(Type elementType)
    {
        ArgumentNullException.ThrowIfNull(elementType);
        return Expression.Parameter(elementType, "elementParam");
    }
}
