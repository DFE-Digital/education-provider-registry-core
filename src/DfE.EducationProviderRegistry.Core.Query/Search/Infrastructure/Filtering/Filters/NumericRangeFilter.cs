using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.Specification.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Shared.Specifications.Comparison;

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
            return LowerPropertyGreaterThanOrEqualTo(range.Lower!.Value)
                .And(UpperPropertyLessThanOrEqualTo(range.Upper!.Value));
        }

        if (range.HasLowerBounds)
        {
            return LowerPropertyGreaterThanOrEqualTo(range.Lower!.Value);
        }

        return UpperPropertyLessThanOrEqualTo(range.Upper!.Value);
    }

    private PropertyGreaterThanOrEqualSpecification<TModel, int> LowerPropertyGreaterThanOrEqualTo(int value) => new(_lowerPropertyTarget, value);

    private PropertyLessThanOrEqualSpecification<TModel, int> UpperPropertyLessThanOrEqualTo(int value) => new(_upperPropertyTarget, value);
}
