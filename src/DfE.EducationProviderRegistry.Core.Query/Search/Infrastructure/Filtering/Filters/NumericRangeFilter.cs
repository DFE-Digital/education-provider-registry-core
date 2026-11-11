using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.Specification.Extensions;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

internal sealed class NumericRangeFilter<TModel> : ISearchFilter<TModel>
{
    private readonly Expression<Func<TModel, int?>> _lowerPropertyTarget;
    private readonly Expression<Func<TModel, int?>> _upperPropertyTarget;

    public NumericRangeFilter(
        Expression<Func<TModel, int?>> lowerPropertyTarget,
        Expression<Func<TModel, int?>> upperPropertyTarget)
    {
        ArgumentNullException.ThrowIfNull(lowerPropertyTarget);
        ArgumentNullException.ThrowIfNull(upperPropertyTarget);

        _lowerPropertyTarget = lowerPropertyTarget;
        _upperPropertyTarget = upperPropertyTarget;
    }

    public ISpecification<TModel> CreateSpecification(SearchFilterRequest request)
    {
        AgeRange filter =
            request.ParseNumericRange();

        BoundedRange<int> range =
            filter.Range;

        if (range.HasLowerAndUpperBounds)
        {
            return _lowerPropertyTarget.LessThanOrEqualTo(range.Lower!.Value)
                .And(_upperPropertyTarget.GreaterThanOrEqualTo(range.Upper!.Value));
        }

        if (range.HasLowerBounds)
        {
            return _lowerPropertyTarget.LessThanOrEqualTo(range.Lower!.Value)
                .And(_upperPropertyTarget.GreaterThanOrEqualTo(range.Lower!.Value));
        }

        return _lowerPropertyTarget
            .LessThanOrEqualTo(range.Upper.Value)
            .And(_upperPropertyTarget.GreaterThanOrEqualTo(range.Upper!.Value));
    }
}
