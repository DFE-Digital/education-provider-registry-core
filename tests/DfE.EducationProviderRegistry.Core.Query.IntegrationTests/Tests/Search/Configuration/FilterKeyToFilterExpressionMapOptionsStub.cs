using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;

internal static class FilterKeyToFilterExpressionMapOptionsStub
{
    private static readonly Dictionary<string, string?> _stubFilter = new()
    {
        {"FilterKeyToFilterExpressionMapOptions:SearchFilterToExpressionMap:STUB:FilterExpressionKey", "STUBBED_FILTER" }
    };

    internal static IConfigurationBuilder StubFilterOptions(this IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(_stubFilter);
        return builder;
    }
}
