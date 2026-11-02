using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Filters;

internal static class FilterKeyToFilterExpressionMapOptionsConfigurationAdaptor
{
    private const string RootConfigurationKey =
        "FilterKeyToFilterExpressionMapOptions:SearchFilterToExpressionMap";

    public static Dictionary<string, string?> ToConfiguration(FilterKeyToFilterExpressionMapOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        Dictionary<string, string?> configuration = [];

        foreach (KeyValuePair<string, FilterExpressionOptions> mapping
                 in options.SearchFilterToExpressionMap)
        {
            configuration[
                $"{RootConfigurationKey}:{mapping.Key}:FilterExpressionKey"] = mapping.Value.FilterExpressionKey;
        }

        return configuration;
    }
}
