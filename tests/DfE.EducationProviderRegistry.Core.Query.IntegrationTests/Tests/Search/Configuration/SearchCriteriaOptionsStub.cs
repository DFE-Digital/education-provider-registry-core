using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;

internal static class SearchCriteriaOptionsStub
{
    private static readonly Dictionary<string, string?> _stubSearchCriteria = new()
    {
            {
                "SearchCriteria:SearchFields:0",
                "Name"
            },
            {
                "SearchCriteria:Facets:0",
                "establishmenttypeid" // FacetDefinition must be defined
            }
    };

    internal static IConfigurationBuilder StubSearchCriteriaOptions(this IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(_stubSearchCriteria);
        return builder;
    }
}

