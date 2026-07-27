using System.Linq.Expressions;
using System.Reflection;
using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

public sealed class PropertyEqualsAnySpecification<TProjection> : ISpecification<TProjection>
{
    private readonly string _propertyName;
    private readonly IReadOnlyCollection<object> _values;

    public PropertyEqualsAnySpecification(
        string propertyName,
        IReadOnlyCollection<object> values)
    {
        _propertyName = propertyName;
        _values = values;
    }

    public bool IsSatisfiedBy(TProjection input)
    {
        return ToExpression().Compile()(input);
    }

    public Expression<Func<TProjection, bool>> ToExpression()
    {
        if (_values.Count == 0)
        {
            return _ => true;
        }

        ParameterExpression parameter =
            Expression.Parameter(
                typeof(TProjection),
                "projection");

        PropertyInfo property =
            typeof(TProjection).GetProperty(_propertyName) ??
                throw new InvalidOperationException($"Property '{_propertyName}' not found.");

        Expression propertyExpression =
            Expression.Property(parameter, property);

        Type propertyType = property.PropertyType;

        object?[] typedValues =
        [
            .. _values
                .Where(v => v != null)
                .Select(v => Convert.ChangeType(v, propertyType))
        ];

        Expression body =
            typedValues
                .Select(v =>
                    (Expression)Expression.Equal(
                        propertyExpression,
                        Expression.Constant(v, propertyType)))
                .Aggregate(Expression.OrElse);

        return Expression.Lambda<Func<TProjection, bool>>(
            body,
            parameter);
    }
}
