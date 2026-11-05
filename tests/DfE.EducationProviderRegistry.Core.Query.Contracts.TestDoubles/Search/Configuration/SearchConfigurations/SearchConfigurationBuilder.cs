using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.SearchConfigurations;

internal sealed class SearchConfigurationBuilder
{
    private readonly List<(
        string term,
        IReadOnlyList<IndexedFieldConfiguration> fieldConfiguration,
        string fieldChainingPredicate)> _behaviour;

    public SearchConfigurationBuilder()
    {
        _behaviour = [];
    }

    public static SearchConfigurationBuilder Create() => new();

    public SearchConfigurationBuilder WithBehaviourForSearchTerm(
        string termKey,
        IReadOnlyList<IndexedFieldConfiguration> fieldConfiguration,
        string fieldChainingPredicate)
    {
        _behaviour.Add(
            (termKey, fieldConfiguration, fieldChainingPredicate));

        return this;
    }

    public SearchConfiguration Build()
    {
        return new SearchConfiguration
        {
            Keys =
            [
                .. _behaviour.Select(behaviour =>
                new SearchIndexKeyConfiguration
                {
                    SearchTermKey = behaviour.term,
                    FieldChainingPredicate = behaviour.fieldChainingPredicate,
                    IndexedFields = behaviour.fieldConfiguration
                })
            ]
        };
    }
}
