using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;

internal sealed class SearchConfigurationBuilder
{
    private readonly List<(string term, IReadOnlyList<IndexedFieldConfiguration> fieldConfiguration, string fieldChainingPredicate)> _behaviour;

    public SearchConfigurationBuilder()
    {
        _behaviour = [];
    }

    public static SearchConfigurationBuilder Create() => new();

    public SearchConfigurationBuilder WithBehaviourForSearchTerm(string termKey, IReadOnlyList<IndexedFieldConfiguration> fieldConfiguration, string fieldChainingPredicate)
    {
        _behaviour.Add(
            item: (termKey, fieldConfiguration, fieldChainingPredicate));

        return this;
    }

    public Dictionary<string, string?> Build()
    {
        string rootConfigurationKey = $"{nameof(SearchConfiguration)}:keys";

        Dictionary<string, string?> output = [];

        for (int index = 0; index < _behaviour.Count; index++)
        {
            (string term, IReadOnlyList<IndexedFieldConfiguration> fieldConfiguration, string chainingPredicate) = _behaviour[index];

            output.Add($"{rootConfigurationKey}:{index}:searchTermKey", term);
            output.Add($"{rootConfigurationKey}:{index}:fieldChainingPredicate", chainingPredicate);

            for (int fieldIndex = 0; fieldIndex < fieldConfiguration.Count; fieldIndex++)
            {
                output.Add($"{rootConfigurationKey}:{index}:indexedFields:{fieldIndex}:fieldName", fieldConfiguration[fieldIndex].FieldName);
                output.Add($"{rootConfigurationKey}:{index}:indexedFields:{fieldIndex}:defaultBehaviourChainingPredicate", fieldConfiguration[fieldIndex].DefaultBehaviourChainingPredicate);

                IReadOnlyList<SearchBehaviourConfiguration> fieldBehaviours = fieldConfiguration[fieldIndex].SearchBehaviours;
                for (int searchBehavioursIndex = 0; searchBehavioursIndex < fieldBehaviours.Count; searchBehavioursIndex++)
                {
                    output.Add($"{rootConfigurationKey}:{index}:indexedFields:{fieldIndex}:searchBehaviours:{searchBehavioursIndex}:name", fieldBehaviours[searchBehavioursIndex].Name);
                    output.Add($"{rootConfigurationKey}:{index}:indexedFields:{fieldIndex}:searchBehaviours:{searchBehavioursIndex}:chainingPredicate", fieldBehaviours[searchBehavioursIndex].ChainingPredicate);
                }
            }
        }
        return output;
    }
}
