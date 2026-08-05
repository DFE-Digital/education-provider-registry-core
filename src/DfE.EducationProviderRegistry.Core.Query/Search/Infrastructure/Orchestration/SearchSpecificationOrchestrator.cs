using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Orchestration.SpecificationChaining;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Orchestration;

public sealed class SearchSpecificationOrchestrator<TEntity>
    : ISearchSpecificationOrchestrator<TEntity>
    where TEntity : class
{
    private readonly ISearchIndexFieldSpecificationOrchestrator<TEntity> _indexFieldOrchestrator;
    private readonly ChainingPredicateRegistry<TEntity> _predicateRegistry;
    private readonly SearchConfiguration _config;

    public SearchSpecificationOrchestrator(
        ISearchIndexFieldSpecificationOrchestrator<TEntity> indexFieldOrchestrator,
        ChainingPredicateRegistry<TEntity> predicateRegistry,
        SearchConfiguration config)
    {
        _indexFieldOrchestrator = indexFieldOrchestrator;
        _predicateRegistry = predicateRegistry;
        _config = config;
    }

    public ISpecification<TEntity> Orchestrate(string key, string value)
    {
        SearchIndexKeyConfiguration keyConfig =
            _config.Keys.First(indexKey =>
                string.Equals(indexKey.SearchTermKey, key, StringComparison.OrdinalIgnoreCase));

        ISpecification<TEntity>? combined = null;

        foreach (IndexedFieldConfiguration fieldConfig in keyConfig.IndexedFields)
        {
            IEnumerable<(string BehaviourName, string? BehaviourPredicate)> behaviours =
                fieldConfig.SearchBehaviours
                    .Select(searchBehaviour =>
                        (searchBehaviour.Name, searchBehaviour.ChainingPredicate));

            ISpecification<TEntity> fieldSpec =
                _indexFieldOrchestrator.Orchestrate(
                    fieldConfig.FieldName,
                    behaviours,
                    fieldConfig.ChainingPredicate,
                    value);

            if (combined is null)
            {
                combined = fieldSpec;
                continue;
            }

            string predicateName =
                fieldConfig.ChainingPredicate
                    ?? throw new InvalidOperationException(
                        $"No chaining predicate defined for field '{fieldConfig.FieldName}'.");

            Func<
                ISpecification<TEntity>,
                ISpecification<TEntity>,
                ISpecification<TEntity>> combiner =
                    _predicateRegistry.Resolve(predicateName);

            combined = combiner(combined, fieldSpec);
        }

        return combined!;
    }
}

