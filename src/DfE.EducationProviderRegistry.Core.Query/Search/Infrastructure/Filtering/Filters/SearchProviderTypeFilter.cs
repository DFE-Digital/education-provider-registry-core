using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public sealed class SearchProviderTypeFilter : ISearchFilter<SearchAggregate>
{
    public ISpecification<SearchAggregate> CreateSpecification(
        SearchFilterRequest request)
    {
        IReadOnlyCollection<long> values =
        [
            .. request.FilterValues
                .Where((filterValue) => filterValue != null)
                .Select((value) => Convert.ToInt64(value))
        ];

        return new PropertyEqualsAnyValuesSpecification<SearchAggregate, long>(
            searchProvider => searchProvider.ProviderTypeId,
            values);
    }
}
