using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Filtering.TestDoubles;

public static class SearchFilterRequestTestDoubles
{
    public static SearchFilterRequest StubFilterValues(params object[] filterValues)
    {
        return new SearchFilterRequest("stub-key", filterValues: filterValues);
    }
}
