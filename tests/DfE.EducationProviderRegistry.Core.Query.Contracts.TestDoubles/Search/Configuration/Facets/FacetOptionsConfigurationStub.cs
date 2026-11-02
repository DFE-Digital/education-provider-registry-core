using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Options;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Facets;

public static class FacetOptionsConfigurationStub
{
    public static FacetOptions Stub()
    {
        Dictionary<string, FacetDefinition> definitions = new()
        {
            {
                "searchprovidertypename",
                new FacetDefinition()
                {
                    ValueProperty = nameof(SearchAggregate.ProviderTypeName),
                    LabelProperty = nameof(SearchAggregate.ProviderTypeName)
                }
            }
        };

        return new()
        {
            Definitions = definitions
        };
    }

    public static FacetOptions StubFor(IEnumerable<KeyValuePair<string, FacetDefinition>> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        return new()
        {
            Definitions =
                definitions.ToDictionary(
                    definition => definition.Key,
                    definition => definition.Value)
        };
    }
}
