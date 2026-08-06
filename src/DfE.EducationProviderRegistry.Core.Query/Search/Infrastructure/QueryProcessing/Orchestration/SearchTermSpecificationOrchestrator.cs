using System.Data;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;

/// <summary>
/// Builds search specifications from one or more search terms.
/// Each term is matched to its configured key, expanded into field‑level specifications,
/// and chained using the key‑level predicate defined in configuration.
/// </summary>
public sealed class SearchTermSpecificationOrchestrator<TEntity> : ISearchTermSpecificationOrchestrator<TEntity>
    where TEntity : class
{
    private readonly ISearchIndexFieldSpecificationOrchestrator<TEntity> _indexFieldOrchestrator;
    private readonly ChainingPredicateRegistry<TEntity> _predicateRegistry;
    private readonly SearchConfiguration _config;

    public SearchTermSpecificationOrchestrator(
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
            _config.Keys.First(searchIndexKeyConfiguration =>
                string.Equals(
                    searchIndexKeyConfiguration.SearchTermKey,
                    key, StringComparison.OrdinalIgnoreCase));

        ISpecification<TEntity>? combined = null;

        foreach (IndexedFieldConfiguration fieldConfig in keyConfig.IndexedFields)
        {
            ArgumentNullException.ThrowIfNull(fieldConfig);
            ArgumentNullException.ThrowIfNullOrEmpty(fieldConfig.ChainingPredicate);
            ArgumentNullException.ThrowIfNull(fieldConfig.SearchBehaviours);

            IEnumerable<(string BehaviourName, string? BehaviourPredicate)> behaviours =
                fieldConfig.SearchBehaviours
                    .Select(behaviourConfiguration =>
                        (behaviourConfiguration.Name, behaviourConfiguration.ChainingPredicate));

            ArgumentNullException.ThrowIfNull(behaviours);

            ISpecification<TEntity> fieldSpec =
                _indexFieldOrchestrator.Orchestrate(
                    fieldConfig.FieldName,
                    behaviours,
                    fieldConfig.ChainingPredicate,
                    value);

            if (fieldConfig.ChainingPredicate == null)
            {
                throw new InvalidOperationException(
                    $"Indev field chaining predicate required for field: {fieldConfig.FieldName}, key: {keyConfig.SearchTermKey}");
            }

            combined = Combine(
                combined,
                fieldSpec,
                fieldConfig.ChainingPredicate);
        }

        return combined!;
    }

    /// <summary>
    /// Combines two specifications using the predicate resolved from the registry.
    /// If the left specification is null, the right specification is returned directly.
    /// </summary>
    private ISpecification<TEntity> Combine(
        ISpecification<TEntity>? left,
        ISpecification<TEntity> right,
        string predicateName)
    {
        Func<
            ISpecification<TEntity>,
            ISpecification<TEntity>,
            ISpecification<TEntity>> combiner =
                _predicateRegistry.Resolve(predicateName);

        return (left is null) ?
            right : combiner(left, right);
    }
}
