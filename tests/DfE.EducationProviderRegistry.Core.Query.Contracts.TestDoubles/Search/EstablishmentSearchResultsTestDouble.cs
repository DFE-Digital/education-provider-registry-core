using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;

[ExcludeFromCodeCoverage]
public static class EstablishmentSearchResultsTestDouble
{
    public static EstablishmentSearchResults Stub()
    {
        return StubCount(3);
    }

    public static EstablishmentSearchResults StubCount(int count)
    {
        List<EstablishmentSearchResult> results = [];

        for (int i = 0; i < count; i++)
        {
            results.Add(
                EstablishmentSearchResultTestDouble.Create());
        }

        return new EstablishmentSearchResults(results);
    }

    public static EstablishmentSearchResults EmptyStub() => new([]);

    public static EstablishmentSearchResults Create(params EstablishmentSearchResult[] results) => new([.. results]);
}
