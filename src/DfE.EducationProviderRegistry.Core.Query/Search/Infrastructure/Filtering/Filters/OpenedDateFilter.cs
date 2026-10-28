using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Specifications.Comparison;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

internal sealed class OpenedDateFilter : ISearchFilter<SearchAggregate>
{
    public ISpecification<SearchAggregate> CreateSpecification(SearchFilterRequest request)
    {
        IReadOnlyCollection<DateOnly> requestFilterValues = SearchFilterRequestParseHelpers.ParseFilterRequestDates(request);

        if (requestFilterValues.Count == 0)
        {
            throw new ArgumentException("No parsed values to filter in OpenDateFilter");
        }

        // the earliest date
        DateOnly greaterThanValue = requestFilterValues.Min();

        return new PropertyComparisonSpecification<SearchAggregate, DateOnly>(
            (aggregate) => aggregate.OpenedDate,
            greaterThanValue,
            ComparisonOperator.GreaterThanOrEqual);
    }
}
