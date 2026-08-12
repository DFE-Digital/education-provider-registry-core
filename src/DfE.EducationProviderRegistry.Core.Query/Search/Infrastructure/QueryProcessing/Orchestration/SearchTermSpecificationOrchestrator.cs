using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;

public sealed class SearchTermSpecificationOrchestrator<TEntity>
    : ISearchTermSpecificationOrchestrator<TEntity>
    where TEntity : class
{
    private readonly ISearchIndexFieldSpecificationOrchestrator<TEntity> _indexFieldOrchestrator;
    private readonly IChainingPredicateRegistry<TEntity> _predicateRegistry;
    private readonly SearchConfiguration _searchConfiguration;

    public SearchTermSpecificationOrchestrator(
        ISearchIndexFieldSpecificationOrchestrator<TEntity> indexFieldOrchestrator,
        IChainingPredicateRegistry<TEntity> predicateRegistry,
        IOptions<SearchConfiguration> searchConfiguration)
    {
        _indexFieldOrchestrator = indexFieldOrchestrator;
        _predicateRegistry = predicateRegistry;
        _searchConfiguration = searchConfiguration.Value;
    }

    public ISpecification<TEntity> Orchestrate(string key, string value)
    {
        SearchIndexKeyConfiguration keyConfig =
            _searchConfiguration.Keys.FirstOrDefault(k =>
                string.Equals(k.SearchTermKey, key, StringComparison.OrdinalIgnoreCase))
            ?? throw new KeyNotFoundException(
                $"Search key '{key}' is not configured in SearchConfiguration.");

        ISpecification<TEntity>? combined = null;

        foreach (IndexedFieldConfiguration fieldConfig in keyConfig.IndexedFields)
        {
            ArgumentNullException.ThrowIfNull(fieldConfig);
            ArgumentNullException.ThrowIfNull(fieldConfig.SearchBehaviours);

            IEnumerable<(string Name, string? ChainingPredicate)> behaviours =
                fieldConfig.SearchBehaviours
                    .Select(searchBehaviourConfiguration =>
                        (searchBehaviourConfiguration.Name,
                        searchBehaviourConfiguration.ChainingPredicate));

            ISpecification<TEntity> fieldSpec =
                _indexFieldOrchestrator.Orchestrate(
                    fieldConfig.FieldName,
                    behaviours,
                    fieldConfig.ChainingPredicate,
                    value);

            combined = _predicateRegistry.Chain(
                combined,
                fieldSpec,
                keyConfig.ChainingPredicate);
        }

        return combined!;
    }
}
