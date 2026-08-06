using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Behaviours.Specifications;

public sealed class PropertyEqualsSpecification<TEntity> : ISpecification<TEntity>
{
    private readonly string _propertyPath;
    private readonly string _value;

    public PropertyEqualsSpecification(string propertyPath, string value)
    {
        ArgumentNullException.ThrowIfNull(propertyPath);
        ArgumentNullException.ThrowIfNull(value);

        _propertyPath = propertyPath;
        _value = value;
    }

    public Expression<Func<TEntity, bool>> ToExpression()
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "e");
        Expression property = ResolvePropertyPath(parameter, _propertyPath);
        ConstantExpression constant = Expression.Constant(_value);
        BinaryExpression equals = Expression.Equal(property, constant);

        return Expression.Lambda<Func<TEntity, bool>>(equals, parameter);
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
