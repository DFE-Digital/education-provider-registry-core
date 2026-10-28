using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.Specification.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Specifications.Comparison;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

internal sealed class DateRangeFilter : ISearchFilter<SearchAggregate>
{
    private readonly Expression<Func<SearchAggregate, DateOnly?>> _propertyTarget;

    public DateRangeFilter(Expression<Func<SearchAggregate, DateOnly?>> propertyTarget)
    {
        ArgumentNullException.ThrowIfNull(propertyTarget);
        _propertyTarget = propertyTarget;
    }

    public ISpecification<SearchAggregate> CreateSpecification(SearchFilterRequest request)
    {
        DateFilterRange filter =
            SearchFilterRequestParseHelpers.ParseFilterRequestDates(request);

        BoundedRange<DateOnly> boundedRange = filter.Range;

        if (boundedRange.HasLowerAndUpperBounds)
        {
            return new PropertyComparisonSpecification<SearchAggregate, DateOnly>(
                        _propertyTarget,
                        filter.Range.Lower!.Value,
                        ComparisonOperator.GreaterThanOrEqual)
                .And(
                    new PropertyComparisonSpecification<SearchAggregate, DateOnly>(
                        _propertyTarget,
                        filter.Range.Upper!.Value,
                        ComparisonOperator.LessThanOrEqual));
        }

        if (boundedRange.HasLowerBounds)
        {
            return new PropertyComparisonSpecification<SearchAggregate, DateOnly>(
                _propertyTarget,
                filter.Range.Lower!.Value,
                ComparisonOperator.GreaterThanOrEqual);
        }

        // filter can only have Upper bounds
        return new PropertyComparisonSpecification<SearchAggregate, DateOnly>(
            _propertyTarget,
            filter.Range.Upper!.Value,
            ComparisonOperator.LessThanOrEqual);
    }
}
