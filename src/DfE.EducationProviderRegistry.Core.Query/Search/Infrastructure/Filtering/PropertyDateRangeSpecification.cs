using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

public sealed class PropertyDateRangeSpecification<TProjection>
    : ISpecification<TProjection>
{
    private readonly Expression<Func<TProjection, DateOnly?>> _selector;
    private readonly DateRange _dateRange;

    public PropertyDateRangeSpecification(
        Expression<Func<TProjection, DateOnly?>> property,
        DateRange dateRange)
    {
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(dateRange);
        _selector = property;
        _dateRange = dateRange;
    }

    public bool IsSatisfiedBy(TProjection input)
    {
        return ToExpression().Compile().Invoke(input);
    }

    public Expression<Func<TProjection, bool>> ToExpression()
    {
        if (!_dateRange.HasBounds)
            return _ => true;

        Expression date = _selector.Body;
        Expression value = Expression.Property(date, nameof(Nullable<DateOnly>.Value));
        Expression predicate = Expression.Property(date, nameof(Nullable<DateOnly>.HasValue));

        if (_dateRange.From is DateOnly from)
        {
            predicate = Expression.AndAlso(
                predicate,
                Expression.GreaterThanOrEqual(value, Expression.Constant(from)));
        }

        if (_dateRange.To is DateOnly to)
        {
            predicate = Expression.AndAlso(
                predicate,
                Expression.LessThanOrEqual(value, Expression.Constant(to)));
        }

        return Expression.Lambda<Func<TProjection, bool>>(predicate, _selector.Parameters);
    }
}
