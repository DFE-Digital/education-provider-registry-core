using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.PropertyPathResolution;

public static class ExpressionPathNavigator
{
    public static Expression Navigate(Expression root, string path)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(path);

        Expression current = root;

        string[] parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
        {
            current = Expression.PropertyOrField(current, part);
        }

        return current;
    }
}
