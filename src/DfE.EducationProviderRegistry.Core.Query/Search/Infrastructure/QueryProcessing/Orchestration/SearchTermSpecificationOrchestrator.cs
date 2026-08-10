using System.Data;
using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;

public sealed class SearchTermSpecificationOrchestrator<TEntity> : ISearchTermSpecificationOrchestrator<TEntity>
    where TEntity : class
{
    private readonly ISearchIndexFieldSpecificationOrchestrator<TEntity> _indexFieldOrchestrator;
    private readonly ChainingPredicateRegistry<TEntity> _predicateRegistry;
    private readonly SearchConfiguration _searchConfiguration;

    public SearchTermSpecificationOrchestrator(
        ISearchIndexFieldSpecificationOrchestrator<TEntity> indexFieldOrchestrator,
        ChainingPredicateRegistry<TEntity> predicateRegistry,
        IOptions<SearchConfiguration> searchConfiguration)
    {
        _indexFieldOrchestrator = indexFieldOrchestrator;
        _predicateRegistry = predicateRegistry;
        _searchConfiguration = searchConfiguration.Value;
    }

    public ISpecification<TEntity> Orchestrate(string key, string value)
    {
        SearchIndexKeyConfiguration keyConfig =
            _searchConfiguration.Keys.FirstOrDefault(searchConfigurationKey =>
                string.Equals(searchConfigurationKey.SearchTermKey, key, StringComparison.OrdinalIgnoreCase)) ??
                throw new KeyNotFoundException($"Search key '{key}' is not configured in SearchConfiguration.");

        ISpecification<TEntity>? combined = null;

        foreach (IndexedFieldConfiguration fieldConfig in keyConfig.IndexedFields)
        {
            ArgumentNullException.ThrowIfNull(fieldConfig);
            ArgumentNullException.ThrowIfNull(fieldConfig.SearchBehaviours);

            IEnumerable<(string BehaviourName, string? BehaviourPredicate)> behaviours =
                fieldConfig.SearchBehaviours
                    .Select(behaviourConfiguration =>
                        (behaviourConfiguration.Name, behaviourConfiguration.ChainingPredicate));

            ISpecification<TEntity> fieldSpec =
                _indexFieldOrchestrator.Orchestrate(
                    fieldConfig.FieldName,
                    behaviours,
                    fieldConfig.ChainingPredicate,
                    value);

            combined = Combine(
                combined,
                fieldSpec,
                keyConfig.ChainingPredicate);
        }

        return combined!;
    }

    private ISpecification<TEntity> Combine(
        ISpecification<TEntity>? left,
        ISpecification<TEntity> right,
        string? predicateName)
    {
        if (string.IsNullOrWhiteSpace(predicateName))
        {
            return right;
        }

        Func<ISpecification<TEntity>,
            ISpecification<TEntity>,
            ISpecification<TEntity>> combiner =
                _predicateRegistry.Resolve(predicateName);

        return (left is null) ? right : combiner(left, right);
    }
}
