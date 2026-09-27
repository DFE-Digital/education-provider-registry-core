using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Criteria;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Filters;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.SearchConfiguration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration;

public sealed class SearchUseCaseConfigurationBuilder
{
    private readonly List<(string termKey, string fieldChainPredicate, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)> _terms;
    private readonly List<KeyValuePair<string, string?>> _filters;
    // TODO should facets be dynamically consumed as part of SearchCriteria
    private readonly List<string> _facets;

    public SearchUseCaseConfigurationBuilder()
    {
        _terms = [];
        _filters = [];
        _facets = [];
    }

    public SearchUseCaseConfigurationBuilder WithSearchTerm(
        string termKey,
        IEnumerable<Action<IndexedFieldConfigurationBuilder>> configureFields,
        ChainFieldsBehaviour fieldChainedBehaviour = ChainFieldsBehaviour.OR)
    {
        string fieldChainingPredicate =
            (fieldChainedBehaviour == ChainFieldsBehaviour.OR ?
                IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE :
                    IndexedFieldConfigurationBuilder.AND_CHAINING_PREDICATE);

        _terms.Add(
            (termKey,
                fieldChainingPredicate,
                    configureFields));

        return this;
    }

    public SearchUseCaseConfigurationBuilder WithSearchTerm(
        string termKey,
        Action<IndexedFieldConfigurationBuilder> configureField,
        ChainFieldsBehaviour fieldChainedBehaviour = ChainFieldsBehaviour.OR) => WithSearchTerm(termKey, [configureField], fieldChainedBehaviour);

    public SearchUseCaseConfigurationBuilder WithRequestFacet(string facet)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(facet);
        _facets.Add(facet);
        return this;
    }

    public SearchUseCaseConfigurationBuilder WithFilter(string filterKey, string concreteFilter)
    {
        _filters.Add(
            new(
                filterKey, concreteFilter));

        return this;
    }

    public SearchUseCaseConfigurationBuilder WithFilters(IEnumerable<KeyValuePair<string, string?>> filter)
    {
        _filters.AddRange(filter);
        return this;
    }

    public static SearchUseCaseConfigurationBuilder Create()
    {
        return new();
    }

    public IReadOnlyDictionary<string, string?> Build()
    {
        // FilterKeyToFilterExpressionMapOptions:SearchFilterToExpressionMap is required
        Dictionary<string, string?> builtFiltersConfiguration =
            FilterKeyToFilterExpressionMapOptionsConfigurationStub.StubFilterOptions(
                _filters.Count > 0 ? _filters : [.. FilterKeyToFilterExpressionMapOptionsConfigurationStub.StubFilter]);

        // SearchConfiguration
        SearchConfigurationBuilder searchConfigBuilder = SearchConfigurationBuilder.Create();

        foreach ((string termKey, string chainFieldsWithPredicate, IEnumerable<Action<IndexedFieldConfigurationBuilder>> fieldsConfigure) in _terms)
        {
            IndexedFieldConfiguration[] fields =
            [.. fieldsConfigure.Select(fieldConfigure =>
                {
                    IndexedFieldConfigurationBuilder builder = IndexedFieldConfigurationBuilder.Create();
                    fieldConfigure.Invoke(builder);
                    return builder.Build();
                })];

            searchConfigBuilder.WithBehaviourForSearchTerm(termKey, fields, fieldChainingPredicate: chainFieldsWithPredicate);
        }

        Dictionary<string, string?> builtSearchConfiguration = searchConfigBuilder.Build();

        return
            //SearchCriteria // TODO configure SearchCriteria required for Facets turn on/off?
            builtFiltersConfiguration.Concat(SearchCriteriaOptionsStub.Stub)
                .Concat(builtSearchConfiguration)
                .ToDictionary();
    }
}
