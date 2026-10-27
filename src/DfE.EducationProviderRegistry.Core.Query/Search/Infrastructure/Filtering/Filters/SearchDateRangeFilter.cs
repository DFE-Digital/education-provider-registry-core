using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public class SearchDateRangeFilter : ISearchFilter<SearchAggregate>
{
    private readonly Expression<Func<SearchAggregate, DateOnly?>> _selector;

    public SearchDateRangeFilter(Expression<Func<SearchAggregate, DateOnly?>> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        _selector = selector;
    }

    public ISpecification<SearchAggregate> CreateSpecification(SearchFilterRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.FilterValues.Length is not 1 || request.FilterValues[0] is not DateRange range)
            throw new ArgumentException("A date filter requires exactly one validated DateRange value.", nameof(request));

        return (range.From, range.To) switch
        {
            (DateOnly from, DateOnly to) =>
                new PropertyBetweenSpecification<SearchAggregate, DateOnly>(_selector, from, to),
            (DateOnly from, null) =>
                new PropertyGreaterThanOrEqualSpecification<SearchAggregate, DateOnly>(_selector, from),
            (null, DateOnly to) =>
                new PropertyLessThanOrEqualSpecification<SearchAggregate, DateOnly>(_selector, to),
            _ => new PropertyBetweenSpecification<SearchAggregate, DateOnly>(_selector, null, null),
        };
    }
}

