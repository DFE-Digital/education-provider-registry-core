using System.Linq.Expressions;
using System.Reflection;
using DfE.Core.Libraries.DesignPatterns.Specification;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Behaviours.Specifications;

internal sealed class IlikeSpecification<TEntity> : ISpecification<TEntity>
{
    private readonly string _propertyPath;
    private readonly string _value;

    public IlikeSpecification(string propertyPath, string value)
    {
        _propertyPath = propertyPath;
        _value = value;
    }

    public Expression<Func<TEntity, bool>> ToExpression()
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");

        Expression property = ResolvePropertyPath(parameter, _propertyPath);

        Expression efFunctions = Expression.Property(null, typeof(EF), nameof(EF.Functions));

        MethodInfo? ilikeMethod =
            typeof(NpgsqlDbFunctionsExtensions)
                .GetMethod(
                    nameof(NpgsqlDbFunctionsExtensions.ILike),
                    [typeof(DbFunctions), typeof(string), typeof(string)]
                );

        Expression pattern = Expression.Constant($"%{_value}%");

        Expression ilikeCall =
            Expression.Call(
                ilikeMethod!,
                efFunctions,
                property,
                pattern);

        return Expression.Lambda<Func<TEntity, bool>>(ilikeCall, parameter);
    }

    public bool IsSatisfiedBy(TEntity input)
    {
        Func<TEntity, bool> compiled = ToExpression().Compile();
        return compiled(input);
    }

    private static Expression ResolvePropertyPath(Expression root, string path)
    {
        Expression current = root;

        foreach (string segment in path.Split('.'))
        {
            current = Expression.PropertyOrField(current, segment);
        }

        return current;
    }
}
