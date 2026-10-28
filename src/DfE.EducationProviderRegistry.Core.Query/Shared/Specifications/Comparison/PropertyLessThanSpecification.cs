using System.Linq.Expressions;

namespace DfE.EducationProviderRegistry.Core.Query.Shared.Specifications.Comparison;

internal sealed class PropertyLessThanSpecification<TProjection, TProperty>
    : PropertyComparisonSpecification<TProjection, TProperty>
    where TProperty : struct, IComparable<TProperty>
{
    public PropertyLessThanSpecification(
        Expression<Func<TProjection, TProperty?>> property,
        TProperty comparator)
        : base(property, comparator)
    {
    }

    protected override BinaryExpression CreateComparison(
        Expression property,
        Expression comparator)
    {
        return Expression.LessThan(property, comparator);
    }
}
