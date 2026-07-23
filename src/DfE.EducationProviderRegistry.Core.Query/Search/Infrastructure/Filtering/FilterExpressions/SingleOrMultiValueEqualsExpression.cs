using System.Linq.Expressions;
using System.Reflection;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;

/// <summary>
/// Builds a predicate expression that checks whether the
/// <c>EstablishmentTypeId</c> property of <typeparamref name="TProjection"/>
/// equals any of the supplied filter values. Supports both single‑value and
/// multi‑value equality semantics.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type the filter expression applies to.
/// </typeparam>
public sealed class SingleOrMultiValueEqualsExpression<TProjection>
    : ISearchFilterExpression<TProjection>
    where TProjection : class
{
    /// <summary>
    /// Creates a predicate expression that evaluates whether the
    /// <c>EstablishmentTypeId</c> property matches any of the values supplied
    /// in the <see cref="SearchFilterRequest"/>.
    /// </summary>
    /// <param name="request">The filter request containing raw filter values.</param>
    /// <returns>
    /// A predicate expression that evaluates to <c>true</c> when the property
    /// matches any of the supplied values, or <c>true</c> when no values are provided.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the target property does not exist on <typeparamref name="TProjection"/>.
    /// </exception>
    public Expression<Func<TProjection, bool>> ToExpression(SearchFilterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        const string PropertyName = "EstablishmentTypeId";

        object[] rawValues = request.FilterValues;

        if (rawValues == null || rawValues.Length == 0)
        {
            return projection => true;
        }

        ParameterExpression parameter =
            Expression.Parameter(typeof(TProjection), "projection");

        PropertyInfo propertyInfo =
            typeof(TProjection).GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException(
                $"Property '{PropertyName}' not found on type '{typeof(TProjection).Name}'.");

        Type propertyType = propertyInfo.PropertyType;

        Expression propertyExpression = Expression.Property(parameter, propertyInfo);

        object?[] typedValues =
            NormalizeFilterValuesToPropertyType(rawValues, propertyType);

        if (typedValues is null || typedValues.Length == 0)
        {
            return projection => true;
        }

        Expression body =
            BuildOrEqualsExpressionChain(
                propertyExpression, typedValues, propertyType);

        return Expression.Lambda<Func<TProjection, bool>>(body, parameter);
    }

    /// <summary>
    /// Converts raw filter values into strongly typed values matching the
    /// property type, ignoring null or whitespace entries.
    /// </summary>
    private static object?[] NormalizeFilterValuesToPropertyType(
        object[] rawValues, Type propertyType) =>
            [.. rawValues
                .Where(value => value != null)
                .Select(value => value.ToString())
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .Select(text => Convert.ChangeType(text, propertyType))
            ];

    /// <summary>
    /// Builds a chain of <c>OR</c> equality expressions comparing the property
    /// to each typed filter value.
    /// </summary>
    private static Expression BuildOrEqualsExpressionChain(
        Expression propertyExpression,
        object?[] typedValues,
        Type propertyType)
    {
        Expression[] equalsExpressions =
            [.. typedValues
                .Select(typedValue =>
                    (Expression)Expression.Equal(
                        propertyExpression,
                        Expression.Constant(typedValue, propertyType)))
            ];

        return equalsExpressions.Aggregate(Expression.OrElse);
    }
}
