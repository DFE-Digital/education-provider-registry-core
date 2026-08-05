using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Orchestration;

public interface ISearchIndexFieldSpecificationOrchestrator<TEntity> where TEntity : class
{
    ISpecification<TEntity> Orchestrate(
        string fieldName,
        IEnumerable<(string BehaviourName, string? BehaviourPredicate)> behaviours,
        string? fieldPredicate,
        string value);
}

