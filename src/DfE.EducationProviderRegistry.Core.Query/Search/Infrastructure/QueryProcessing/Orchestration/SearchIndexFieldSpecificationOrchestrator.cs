using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;

public sealed class SearchIndexFieldSpecificationOrchestrator<TEntity>
    : ISearchIndexFieldSpecificationOrchestrator<TEntity>
    where TEntity : class
{
    private readonly ISearchBehaviourRegistry<TEntity> _behaviourRegistry;
    private readonly IChainingPredicateRegistry<TEntity> _predicateRegistry;

    public SearchIndexFieldSpecificationOrchestrator(
        ISearchBehaviourRegistry<TEntity> behaviourRegistry,
        IChainingPredicateRegistry<TEntity> predicateRegistry)
    {
        _behaviourRegistry = behaviourRegistry;
        _predicateRegistry = predicateRegistry;
    }

    public ISpecification<TEntity> Orchestrate(
        string fieldName,
        IEnumerable<(string BehaviourName, string BehaviourPredicate)> behaviours,
        string fieldPredicate,
        string value)
    {
        ArgumentNullException.ThrowIfNull(behaviours);

        List<(string BehaviourName, string BehaviourPredicate)> behaviourList = behaviours.ToList();

        if (behaviourList.Count == 0)
        {
            throw new InvalidOperationException(
                $"At least one behaviour must be configured for field '{fieldName}'.");
        }

        ISpecification<TEntity>? combined = null;

        foreach ((string? behaviourName, string? behaviourPredicate) in behaviourList)
        {
            ISpecification<TEntity> spec =
                _behaviourRegistry.ResolveBehaviourSpec(
                    behaviourName,
                    fieldName,
                    value);

            string predicateToUse = behaviourPredicate ?? fieldPredicate;

            combined = _predicateRegistry.Chain(
                combined,
                spec,
                predicateToUse);
        }

        return combined!;
    }
}
