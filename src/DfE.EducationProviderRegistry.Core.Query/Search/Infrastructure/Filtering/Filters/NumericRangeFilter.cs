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
            return LowerPropertyGreaterThanOrEqualTo(range.Lower!.Value)
                .And(UpperPropertyLessThanOrEqualTo(range.Upper!.Value));
        }

        if (range.HasLowerBounds)
        {
            return LowerPropertyGreaterThanOrEqualTo(range.Lower!.Value);
        }

        return UpperPropertyLessThanOrEqualTo(range.Upper!.Value);
    }

    private PropertyGreaterThanOrEqualSpecification<SearchAggregate, int> LowerPropertyGreaterThanOrEqualTo(int value) => new(_lowerPropertyTarget, value);

    private PropertyLessThanOrEqualSpecification<SearchAggregate, int> UpperPropertyLessThanOrEqualTo(int value) => new(_upperPropertyTarget, value);
}
