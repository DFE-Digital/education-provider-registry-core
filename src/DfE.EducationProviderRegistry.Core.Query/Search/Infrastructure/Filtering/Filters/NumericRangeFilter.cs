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
        BoundedRange<int> range = request.ParseNumericRange();

        if (range.HasLowerAndUpperBounds)
        {
            return _lowerPropertyTarget
                .GreaterThanOrEqualTo(range.Lower!.Value)
                .And(_upperPropertyTarget.LessThanOrEqualTo(range.Upper!.Value));
        }

        if (range.HasLowerBounds)
        {
            return _lowerPropertyTarget.GreaterThanOrEqualTo(range.Lower!.Value);
        }

        return _upperPropertyTarget.LessThanOrEqualTo(range.Upper!.Value);
    }
}
