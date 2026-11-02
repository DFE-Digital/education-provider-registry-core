using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Facets;

public static class FacetOptionsConfigurationAdaptor
{
    public static Dictionary<string, string?> ToConfiguration(FacetOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        Dictionary<string, string?> configuration = [];

        foreach (KeyValuePair<string, FacetDefinition> definition in options.Definitions)
        {
            configuration[$"{nameof(FacetOptions)}:{nameof(FacetOptions.Definitions)}:{definition.Key}:{nameof(FacetDefinition.ValueProperty)}"] =
                definition.Value.ValueProperty;

            configuration[$"{nameof(FacetOptions)}:{nameof(FacetOptions.Definitions)}:{definition.Key}:{nameof(FacetDefinition.LabelProperty)}"] =
                definition.Value.LabelProperty;
        }

        return configuration;
    }
}
