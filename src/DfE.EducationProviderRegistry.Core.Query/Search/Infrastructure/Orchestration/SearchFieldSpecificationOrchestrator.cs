using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Orchestration.SpecificationChaining;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Orchestration;

public sealed class SearchIndexFieldSpecificationOrchestrator<TEntity>
    : ISearchIndexFieldSpecificationOrchestrator<TEntity>
    where TEntity : class
{
    private readonly SearchBehaviourRegistry<TEntity> _behaviourRegistry;
    private readonly ChainingPredicateRegistry<TEntity> _predicateRegistry;

    public SearchIndexFieldSpecificationOrchestrator(
        SearchBehaviourRegistry<TEntity> behaviourRegistry,
        ChainingPredicateRegistry<TEntity> predicateRegistry)
    {
        _behaviourRegistry = behaviourRegistry;
        _predicateRegistry = predicateRegistry;
    }

    public ISpecification<TEntity> Orchestrate(
        string fieldName,
        IEnumerable<(string BehaviourName, string? BehaviourPredicate)> behaviours,
        string? fieldPredicate,
        string value)
    {
        ISpecification<TEntity>? combined = null;

        foreach ((string behaviourName, string? behaviourPredicate) in behaviours)
        {
            ISearchBehaviour<TEntity> behaviour =
                _behaviourRegistry.Get(behaviourName);

            ISpecification<TEntity> spec =
                behaviour.Build(fieldName, value);

            if (combined is null)
            {
                combined = spec;
                continue;
            }

            Func<
                ISpecification<TEntity>,
                ISpecification<TEntity>,
                ISpecification<TEntity>> combiner =
                    _predicateRegistry.Resolve(behaviourPredicate ?? fieldPredicate);

            combined = combiner(combined, spec);

        }

        return combined!;
    }
}

