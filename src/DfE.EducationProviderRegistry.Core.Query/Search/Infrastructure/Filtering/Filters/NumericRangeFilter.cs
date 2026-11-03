using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.Specification.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Shared.Specifications.Comparison;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

internal sealed class NumericRangeFilter : ISearchFilter<SearchAggregate>
{
    private readonly Expression<Func<SearchAggregate, int?>> _lowerPropertyTarget;
    private readonly Expression<Func<SearchAggregate, int?>> _upperPropertyTarget;

    public NumericRangeFilter(
        Expression<Func<SearchAggregate, int?>> lowerPropertyTarget,
        Expression<Func<SearchAggregate, int?>> upperPropertyTarget)
    {
        ArgumentNullException.ThrowIfNull(lowerPropertyTarget);
        ArgumentNullException.ThrowIfNull(upperPropertyTarget);

        _lowerPropertyTarget = lowerPropertyTarget;
        _upperPropertyTarget = upperPropertyTarget;
    }

    public ISpecification<SearchAggregate> CreateSpecification(SearchFilterRequest request)
    {
        BoundedRange<int> range = request.ParseNumericRange();

        if (range.HasLowerAndUpperBounds)
        {
            return LowerPropertyLessThanOrEqualTo(range.Lower!.Value)
                .And(UpperPropertyGreaterThanOrEqualTo(range.Upper!.Value));
        }

        if (range.HasLowerBounds)
        {
            return LowerPropertyLessThanOrEqualTo(range.Lower!.Value);
        }

        return UpperPropertyGreaterThanOrEqualTo(range.Upper!.Value);
    }

    private PropertyLessThanOrEqualSpecification<SearchAggregate, int> LowerPropertyLessThanOrEqualTo(int value) =>
        new(_lowerPropertyTarget, value);

    private PropertyGreaterThanOrEqualSpecification<SearchAggregate, int> UpperPropertyGreaterThanOrEqualTo(int value) =>
        new(_upperPropertyTarget, value);
}
