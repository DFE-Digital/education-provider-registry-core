using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.FilterExpressions;

public interface ISearchFilterExpression<TProjection>
{
    ISpecification<TProjection> CreateSpecification(SearchFilterRequest request);
}
