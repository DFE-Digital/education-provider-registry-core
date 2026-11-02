using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Shared.Specifications.Contains;

public sealed class PropertyContainsAnyValuesSpecification<TProjection, TProperty>
    : ISpecification<TProjection>
{
    private readonly Expression<Func<TProjection, IEnumerable<TProperty>?>> _property;
    private readonly IReadOnlyCollection<TProperty> _values;

    public PropertyContainsAnyValuesSpecification(
        Expression<Func<TProjection, IEnumerable<TProperty>?>> property,
        IReadOnlyCollection<TProperty> values)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(values);

        _property = property;
        _values = values;
    }

    public bool IsSatisfiedBy(TProjection input)
    {
        return ToExpression().Compile().Invoke(input);
    }

    public Expression<Func<TProjection, bool>> ToExpression()
    {
        if (_values.Count == 0)
        {
            return _ => true;
        }

        ParameterExpression parameter = _property.Parameters[0];

        Expression propertyIsNotNull =
            Expression.NotEqual(
                _property.Body,
                Expression.Constant(null, _property.Body.Type));

        Expression containsAnyValue = _values
            .Select(value =>
                (Expression)Expression.Call(
                    typeof(Enumerable),
                    nameof(Enumerable.Contains),
                    [typeof(TProperty)],
                    _property.Body,
                    Expression.Constant(value, typeof(TProperty))))
            .Aggregate(Expression.OrElse);

        Expression body = Expression.AndAlso(
            propertyIsNotNull,
            containsAnyValue);

        return Expression.Lambda<Func<TProjection, bool>>(body, parameter);
    }
}
