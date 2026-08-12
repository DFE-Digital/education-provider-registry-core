using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions.Factories;

public interface ISearchFilterSpecificationFactory<TProjection>
    where TProjection : class
{
    ISpecification<TProjection> Create(
        string filterName,
        SearchFilterRequest request);
}
