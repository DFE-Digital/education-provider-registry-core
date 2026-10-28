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
        DateRange filter =
            SearchFilterRequestParseHelpers.ParseFilterRequestDates(request);

        BoundedRange<DateOnly> range = filter.Range;

        if (range.HasLowerAndUpperBounds)
        {
            return GreaterThanOrEqualTo(range.Lower!.Value)
                .And(LessThanOrEqualTo(range.Upper!.Value));
        }

        if (range.HasLowerBounds)
        {
            return GreaterThanOrEqualTo(range.Lower!.Value);
        }

        return LessThanOrEqualTo(range.Upper!.Value);
    }

    private PropertyGreaterThanOrEqualSpecification<SearchAggregate, DateOnly> GreaterThanOrEqualTo(DateOnly value) => new(_propertyTarget, value);
    private PropertyLessThanOrEqualSpecification<SearchAggregate, DateOnly> LessThanOrEqualTo(DateOnly value) => new(_propertyTarget, value);
}
