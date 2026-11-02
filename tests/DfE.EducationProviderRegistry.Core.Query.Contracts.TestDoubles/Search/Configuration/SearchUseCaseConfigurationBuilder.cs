using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Criteria;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Facets;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Filters;
using DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.SearchConfigurations;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Options;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration;

public sealed class SearchUseCaseConfigurationBuilder
{
    private readonly List<(string termKey, string fieldChainPredicate, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)> _terms;
    private readonly List<KeyValuePair<string, string?>> _filters;
    private readonly List<string> _facets;
    private readonly Dictionary<string, FacetDefinition> _facetDefinitions;

    public SearchUseCaseConfigurationBuilder()
    {
        _terms = [];
        _filters = [];
        _facets = [];
        _facetDefinitions = [];
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

    public SearchUseCaseConfigurationBuilder WithFacet(string facet)
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

    public SearchUseCaseConfigurationBuilder WithFacetDefinition(string facet, string valueProperty, string? labelProperty = null)
    {
        _facetDefinitions[facet] = new FacetDefinition()
        {
            ValueProperty = valueProperty,
            LabelProperty = labelProperty,
        };

        return this;
    }

    public static SearchUseCaseConfigurationBuilder Create()
    {
        return new();
    }

    public IReadOnlyDictionary<string, string?> Build()
    {
        // FilterKeyToFilterExpressionMapOptions:SearchFilterToExpressionMap is required
        FilterKeyToFilterExpressionMapOptions filterOptions =
            _filters.Count > 0
                ? FilterKeyToFilterExpressionMapOptionsStub.Stub(_filters)
                : FilterKeyToFilterExpressionMapOptionsStub.Stub();


        // SearchConfiguration
        SearchConfigurationBuilder searchConfigurationBuilder = SearchConfigurationBuilder.Create();

        foreach ((string termKey, string chainFieldsWithPredicate, IEnumerable<Action<IndexedFieldConfigurationBuilder>> fieldsConfigure)
            in _terms)
        {
            IndexedFieldConfiguration[] fields =
            [
                .. fieldsConfigure.Select(fieldConfigure =>
                {
                    IndexedFieldConfigurationBuilder builder =
                        IndexedFieldConfigurationBuilder.Create();

                    fieldConfigure.Invoke(builder);

                    return builder.Build();
                })
            ];

            searchConfigurationBuilder.WithBehaviourForSearchTerm(termKey, fields, fieldChainingPredicate: chainFieldsWithPredicate);
        }

        SearchConfiguration searchConfiguration = searchConfigurationBuilder.Build();

        // FacetOptions
        // FacetOptions:Definitions is required for every requested facet
        FacetOptions facetOptions =
            _facetDefinitions.Count == 0 ?
                FacetOptionsConfigurationStub.Stub() :
                FacetOptionsConfigurationStub.StubFor(_facetDefinitions);

        // SearchCriteria
        // SearchCriteria:Facets is required for every requested facet
        SearchCriteria searchCriteria =
            _facets.Count == 0 ?
                SearchCriteriaOptionsStub.Stub() :
                SearchCriteriaOptionsStub.StubFor(_facets);

        return
            ObjectToConfigurationAdaptor.ToConfiguration(filterOptions, rootConfigurationKey: nameof(FilterKeyToFilterExpressionMapOptions))
                .Concat(ObjectToConfigurationAdaptor.ToConfiguration(facetOptions, nameof(FacetOptions)))
                .Concat(ObjectToConfigurationAdaptor.ToConfiguration(searchCriteria, nameof(SearchCriteria)))
                .Concat(ObjectToConfigurationAdaptor.ToConfiguration(searchConfiguration, nameof(SearchConfiguration)))
                .ToDictionary();
    }
}
