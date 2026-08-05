
using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Orchestration;

public interface ISearchSpecificationOrchestrator<TEntity> where TEntity : class
{
    ISpecification<TEntity> Orchestrate(string key, string value);
}
