using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;

internal static class FilterKeyToFilterExpressionMapOptionsStub
{
    private const string rootConfigurationKey = "FilterKeyToFilterExpressionMapOptions:SearchFilterToExpressionMap";

    internal static readonly Dictionary<string, string?> StubFilter = new()
    {
        {$"{rootConfigurationKey}:STUB:FilterExpressionKey", "STUBBED_FILTER" }
    };

    internal static IConfigurationBuilder StubFilterOptions(this IConfigurationBuilder builder, IEnumerable<KeyValuePair<string, string?>> filterKeyToFilterMapping)
    {
        Dictionary<string, string?> configuration =
            filterKeyToFilterMapping.ToDictionary(
                keySelector: (t) => $"{rootConfigurationKey}:{t.Key}:FilterExpressionKey",
                elementSelector: (t) => t.Value);

        builder.AddInMemoryCollection(configuration);

        return builder;
    }
}
