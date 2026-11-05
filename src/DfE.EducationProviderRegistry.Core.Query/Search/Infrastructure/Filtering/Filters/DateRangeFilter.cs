using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.Specification.Extensions;
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
        DateRange filter =
            request.ParseDateRange();

        BoundedRange<DateOnly> range = filter.Range;

        if (range.HasLowerAndUpperBounds)
        {

            return _propertyTarget.GreaterThanOrEqualTo(range.Lower!.Value)
                .And(_propertyTarget.LessThanOrEqualTo(range.Upper!.Value));
        }

        if (range.HasLowerBounds)
        {
            return _propertyTarget.GreaterThanOrEqualTo(range.Lower!.Value);
        }

        return _propertyTarget.LessThanOrEqualTo(range.Upper!.Value);
    }
}
