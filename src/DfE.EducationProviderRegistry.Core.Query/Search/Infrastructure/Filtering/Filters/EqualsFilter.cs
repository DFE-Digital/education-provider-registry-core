using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Shared.Specifications.Equals;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public sealed class EqualsFilter : ISearchFilter<SearchAggregate>
{
    private readonly Expression<Func<SearchAggregate, object?>> _propertySelector;

    public EqualsFilter(Expression<Func<SearchAggregate, object?>> propertySelector)
    {
        ArgumentNullException.ThrowIfNull(propertySelector);
        _propertySelector = propertySelector;
    }

    public ISpecification<SearchAggregate> CreateSpecification(SearchFilterRequest request)
    {
        return new PropertyEqualsAnyValuesSpecification<SearchAggregate, object?>(_propertySelector, request.FilterValues);
    }
}
