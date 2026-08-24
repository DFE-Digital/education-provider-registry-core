using System.Linq.Expressions;
using System.Reflection;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public static class PropertyPathResolver
{
    public static ResolvedPath Resolve<TEntity>(string path)
    {
        ParsedPropertyPath parsed = PropertyPathParser.Parse(path);

        ParameterExpression rootParam =
            ExpressionParameterFactory
                .CreateRootParameter<TEntity>();

        return parsed.IsCollection
            ? ResolveCollectionPath<TEntity>(parsed, rootParam)
            : ResolveScalarPath(parsed, rootParam);
    }

    private static ResolvedPath ResolveScalarPath(
        ParsedPropertyPath parsed,
        ParameterExpression rootParam)
    {
        Expression access =
            ExpressionPathNavigator
                .Navigate(rootParam, parsed.NavigationName);

        return new ResolvedPath(
            rootParam,
            access,
            false,
            null,
            null
        );
    }

    private static ResolvedPath ResolveCollectionPath<TEntity>(
        ParsedPropertyPath parsed,
        ParameterExpression rootParam)
    {
        Type elementType =
            GetCollectionElementType<TEntity>(parsed.NavigationName);

        ParameterExpression elementParam =
            ExpressionParameterFactory
                .CreateElementParameter(elementType);

        Expression elementAccess =
            ExpressionPathNavigator
                .Navigate(elementParam, parsed.RemainderPath);

        return new ResolvedPath(
            rootParam,
            elementAccess,
            true,
            elementParam,
            parsed.NavigationName
        );
    }

    private static Type GetCollectionElementType<TEntity>(string navigationName)
    {
        PropertyInfo navProp =
            typeof(TEntity).GetProperty(navigationName)
            ?? throw new InvalidOperationException(
                $"Navigation '{navigationName}' not found on {typeof(TEntity).Name}.");

        return navProp.PropertyType.GetGenericArguments()[0];
    }
}
