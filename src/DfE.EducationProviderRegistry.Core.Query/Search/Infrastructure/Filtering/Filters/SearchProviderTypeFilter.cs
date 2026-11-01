using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Shared.Specifications.Equals;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public sealed class SearchProviderTypeFilter : ISearchFilter<SearchAggregate>
{
    public ISpecification<SearchAggregate> CreateSpecification(SearchFilterRequest request)
    {
        return new PropertyEqualsAnyValuesSpecification<SearchAggregate, string?>(
            searchProvider => searchProvider.ProviderTypeName,
            request.FilterValues.Cast<string?>().ToArray());
    }
}
