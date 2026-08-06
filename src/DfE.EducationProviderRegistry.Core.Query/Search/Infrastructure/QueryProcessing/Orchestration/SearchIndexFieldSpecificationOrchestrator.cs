using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;

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
        string fieldPredicate,
        string value)
    {
        if (behaviours is null)
        {
            throw new ArgumentNullException(nameof(behaviours),
                $"Behaviour collection cannot be null for field '{fieldName}'.");
        }

        List<(string BehaviourName, string? BehaviourPredicate)> behaviourList = [.. behaviours];

        if (behaviourList.Count == 0)
        {
            throw new InvalidOperationException(
                $"At least one behaviour must be configured for field '{fieldName}'.");
        }

        ISpecification<TEntity>? combined = null;

        for (int i = 0; i < behaviourList.Count; i++)
        {
            (string behaviourName, string? behaviourPredicate) = behaviourList[i];

            if (string.IsNullOrWhiteSpace(behaviourName))
            {
                throw new InvalidOperationException(
                    $"Behaviour name cannot be null or empty for field '{fieldName}'.");
            }

            ISearchBehaviour<TEntity> behaviour =
                _behaviourRegistry.Get(behaviourName);

            ISpecification<TEntity> spec =
                behaviour.Build(fieldName, value);

            bool isLast = i == behaviourList.Count - 1;

            if (combined is null)
            {
                combined = spec;
                continue;
            }

            if (!isLast)
            {
                string predicateToUse =
                    behaviourPredicate ?? fieldPredicate;

                Func<
                    ISpecification<TEntity>,
                    ISpecification<TEntity>,
                    ISpecification<TEntity>> combiner =
                        _predicateRegistry.Resolve(predicateToUse);

                combined = combiner(combined, spec);
                continue;
            }
        }

        return combined!;
    }
}
