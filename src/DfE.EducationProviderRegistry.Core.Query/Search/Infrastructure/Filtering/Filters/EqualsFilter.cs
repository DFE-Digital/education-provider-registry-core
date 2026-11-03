using System.Linq.Expressions;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Shared.Specifications.Equals;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public sealed class EqualsFilter<TModel> : ISearchFilter<TModel>
{
    private readonly Expression<Func<TModel, object?>> _propertySelector;

    public EqualsFilter(Expression<Func<TModel, object?>> propertySelector)
    {
        ArgumentNullException.ThrowIfNull(propertySelector);
        _propertySelector = propertySelector;
    }

    public ISpecification<TModel> CreateSpecification(SearchFilterRequest request)
    {
        return new PropertyEqualsAnyValuesSpecification<TModel, object?>(_propertySelector, request.FilterValues);
    }
}
