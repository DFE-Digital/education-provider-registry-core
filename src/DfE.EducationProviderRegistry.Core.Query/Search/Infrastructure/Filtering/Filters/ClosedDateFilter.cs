using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.Specification.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Specifications.Comparison;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

internal sealed class ClosedDateFilter : ISearchFilter<SearchAggregate>
{
    public ISpecification<SearchAggregate> CreateSpecification(SearchFilterRequest request)
    {
        (DateOnly from, DateOnly to) =
            SearchFilterRequestParseHelpers.ParseFilterRequestDates(request);

        return new PropertyComparisonSpecification<SearchAggregate, DateOnly>(
                    (aggregate) => aggregate.ClosedDate,
                    from,
                    ComparisonOperator.GreaterThanOrEqual)
            .And(
                new PropertyComparisonSpecification<SearchAggregate, DateOnly>(
                    (aggregate) => aggregate.ClosedDate,
                    to,
                    ComparisonOperator.LessThanOrEqual));
    }
}
