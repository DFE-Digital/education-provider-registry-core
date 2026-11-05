using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Shared.Specifications.Contains;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

public sealed class SenFilter : ISearchFilter<SearchAggregate>
{
    public ISpecification<SearchAggregate> CreateSpecification(
        SearchFilterRequest request)
    {
        return new PropertyContainsAnyValuesSpecification<SearchAggregate, string>(
            searchAggregate => searchAggregate.Sen,
            request.FilterValues.Cast<string>().ToArray());
    }
}
