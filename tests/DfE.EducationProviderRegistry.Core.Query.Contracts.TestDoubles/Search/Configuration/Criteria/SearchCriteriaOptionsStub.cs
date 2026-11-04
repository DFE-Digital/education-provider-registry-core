using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Criteria;

internal static class SearchCriteriaOptionsStub
{
    public static SearchCriteria Stub() => new()
    {
        SearchFields = ["TypeName"],
        Facets = ["providertypename"],
    };

    public static SearchCriteria StubFor(IEnumerable<string> facets)
    {
        return new SearchCriteria()
        {
            SearchFields = ["TypeName"],
            Facets = facets?.ToList() ?? []
        };
    }
}
