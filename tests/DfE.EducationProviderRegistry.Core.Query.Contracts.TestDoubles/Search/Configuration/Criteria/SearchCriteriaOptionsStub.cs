using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Criteria;

internal static class SearchCriteriaOptionsStub
{
    public static SearchCriteria Stub() => new()
    {
        SearchFields = ["TypeName"],
        Facets = ["searchprovidertypename"],
    };
}
