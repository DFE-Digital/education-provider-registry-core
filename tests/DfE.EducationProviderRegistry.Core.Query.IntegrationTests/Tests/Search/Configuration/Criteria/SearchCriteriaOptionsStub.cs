namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration.Criteria;

internal static class SearchCriteriaOptionsStub
{
    public static readonly Dictionary<string, string?> Stub = new()
        {
                {
                    "SearchCriteria:SearchFields:0",
                    "TypeName"
                },
                {
                    "SearchCriteria:Facets:0",
                    "searchprovidertypeid" // FacetDefinition must be defined
                }
        };
}
