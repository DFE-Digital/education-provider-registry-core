using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

internal static class SearchTermTestDouble
{
    public static SearchTerm Stub()
    {
        return new SearchTerm(Key: "name", Value: "test-value");
    }

    public static SearchTerm Stub(string fieldName, string value)
    {
        return new SearchTerm(Key: fieldName, Value: value);
    }

    public static IReadOnlyCollection<SearchTerm?> Empty()
    {
        return [];
    }

    public static IReadOnlyCollection<SearchTerm?> StubSingle()
    {
        return
        [SearchTermTestDouble.Stub()];
    }

    public static IReadOnlyCollection<SearchTerm?> StubMultiple()
    {
        return
        [
            SearchTermTestDouble.Stub("name", "test-name"),
            SearchTermTestDouble.Stub("ukprn", "12345678")
        ];
    }
}
