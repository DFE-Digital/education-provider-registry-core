using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;

internal static class SearchConfigurationExtensions
{
    private static Dictionary<string, string?> Default => new()
        {
            {
                "FilterKeyToFilterExpressionMapOptions:SearchFilterToExpressionMap:type:FilterExpressionKey",
                "EstablishmentTypeFilter"
            },
            {
                "SearchCriteria:SearchFields:0",
                "Name"
            }
            //{
            //    "SearchCriteria:Facets:0",
            //    "Name"
            //}
        };

    internal static IConfigurationBuilder AddDefaultSearchConfiguration(this IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(Default);
        return builder;
    }
}
