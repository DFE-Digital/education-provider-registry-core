using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Specifications.Comparison;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

internal sealed class ClosedDateFilter : ISearchFilter<SearchAggregate>
{
    public ISpecification<SearchAggregate> CreateSpecification(SearchFilterRequest request)
    {
        IReadOnlyCollection<DateOnly> requestFilterValues = SearchFilterRequestParseHelpers.ParseFilterRequestDates(request);

        if (requestFilterValues.Count == 0)
        {
            throw new ArgumentException("No parsed values to filter in ClosedDateFilter");
        }

        // the latest date
        DateOnly lessThanValue = requestFilterValues.Max();

        return new PropertyComparisonSpecification<SearchAggregate, DateOnly>(
            (aggregate) => aggregate.ClosedDate,
            lessThanValue,
            ComparisonOperator.LessThanOrEqual);
    }
}
