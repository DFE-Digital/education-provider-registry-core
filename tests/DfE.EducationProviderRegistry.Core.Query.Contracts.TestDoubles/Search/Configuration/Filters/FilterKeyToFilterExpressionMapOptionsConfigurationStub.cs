namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Filters;

internal static class FilterKeyToFilterExpressionMapOptionsConfigurationStub
{
    private const string RootConfigurationKey = "FilterKeyToFilterExpressionMapOptions:SearchFilterToExpressionMap";

    public static readonly Dictionary<string, string?> StubFilter = new()
        {
            {$"{RootConfigurationKey}:STUB:FilterExpressionKey", "STUBBED_FILTER" }
        };

    public static Dictionary<string, string?> StubFilterOptions(IEnumerable<KeyValuePair<string, string?>> filterKeyToFilterMapping)
    {
        Dictionary<string, string?> configuration =
            filterKeyToFilterMapping.ToDictionary(
                keySelector: (t) => $"{RootConfigurationKey}:{t.Key}:FilterExpressionKey",
                elementSelector: (t) => t.Value);

        return configuration;
    }
}
