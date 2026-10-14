using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

internal static class SearchTermTestDouble
{
    public static SearchTermRequest Stub()
    {
        return new SearchTermRequest(Key: "name", Value: "test-value");
    }

    public static SearchTermRequest Stub(string fieldName, string value)
    {
        return new SearchTermRequest(Key: fieldName, Value: value);
    }

    public static IReadOnlyCollection<SearchTermRequest> Empty()
    {
        return [];
    }

    public static IReadOnlyCollection<SearchTermRequest> StubSingle()
    {
        return
        [SearchTermTestDouble.Stub()];
    }

    public static IReadOnlyCollection<SearchTermRequest> StubMultiple()
    {
        return
        [
            SearchTermTestDouble.Stub("name", "test-name"),
            SearchTermTestDouble.Stub("ukprn", "12345678")
        ];
    }
}
