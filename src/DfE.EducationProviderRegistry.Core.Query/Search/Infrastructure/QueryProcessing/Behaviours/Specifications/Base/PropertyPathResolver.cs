using System.Linq.Expressions;
using System.Reflection;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.Base;

public static class PropertyPathResolver
{
    public sealed class ResolvedPath
    {
        public required ParameterExpression RootParameter { get; init; }
        public required Expression AccessExpression { get; init; }
        public required bool IsCollection { get; init; }
        public ParameterExpression? CollectionElementParameter { get; init; }
        public string? CollectionNavigationName { get; init; }
    }

    public static ResolvedPath Resolve<TEntity>(string fieldPath)
    {
        ParameterExpression rootParam = Expression.Parameter(typeof(TEntity), "e");

        if (!fieldPath.Contains("[]"))
        {
            return new ResolvedPath
            {
                RootParameter = rootParam,
                AccessExpression = BuildNested(rootParam, fieldPath),
                IsCollection = false
            };
        }

        int idx = fieldPath.IndexOf("[]", StringComparison.Ordinal);
        string navigationName = fieldPath[..idx];
        string remainder = fieldPath[(idx + 3)..]; // skip "[]."

        PropertyInfo navProp = typeof(TEntity).GetProperty(navigationName)
            ?? throw new InvalidOperationException($"Navigation '{navigationName}' not found.");

        Type elementType = navProp.PropertyType.GetGenericArguments()[0];
        ParameterExpression elementParam = Expression.Parameter(elementType, "s");

        Expression elementAccess = BuildNested(elementParam, remainder);

        return new ResolvedPath
        {
            RootParameter = rootParam,
            AccessExpression = elementAccess,
            IsCollection = true,
            CollectionElementParameter = elementParam,
            CollectionNavigationName = navigationName
        };
    }

    private static Expression BuildNested(Expression root, string path)
    {
        Expression current = root;

        foreach (string part in path.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            current = Expression.PropertyOrField(current, part);
        }

        return current;
    }
}
