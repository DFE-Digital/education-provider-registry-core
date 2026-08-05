
using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Orchestration;

public interface ISearchSpecificationOrchestrator
{
    ISpecification<TEntity> Orchestrate<TEntity>(
        string key, string value) where TEntity : class;
}
