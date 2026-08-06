using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;

public interface ISearchTermSpecificationOrchestrator<TEntity> where TEntity : class
{
    ISpecification<TEntity> Orchestrate(string key, string value);
}
