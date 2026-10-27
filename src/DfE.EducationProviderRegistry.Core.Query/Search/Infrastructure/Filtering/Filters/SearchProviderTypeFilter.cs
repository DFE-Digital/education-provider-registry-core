using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public sealed class SearchProviderTypeFilter : ISearchFilter<SearchAggregate>
{
    public ISpecification<SearchAggregate> CreateSpecification(
        SearchFilterRequest request)
    {
        IReadOnlyCollection<string?> values =
        [
            .. request.FilterValues
                .Where((filterValue) => filterValue != null)
                .Select((value) => value.ToString())
        ];

        return new PropertyEqualsAnyValuesSpecification<SearchAggregate, string?>(
            searchProvider => searchProvider.ProviderTypeName,
            values);
    }
}
