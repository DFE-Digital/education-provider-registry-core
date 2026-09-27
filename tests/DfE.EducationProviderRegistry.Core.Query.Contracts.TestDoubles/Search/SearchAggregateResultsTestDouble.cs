using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;

[ExcludeFromCodeCoverage]
public static class SearchAggregateResultsTestDouble
{
    public static SearchAggregateResults Stub(int count = 10)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value: count, other: 1);

        List<SearchAggregateResult> searchResults = [];

        for (int i = 0; i < count; i++)
        {
            searchResults.Add(
                SearchAggregateResultTestDouble.Fake());
        }

        return new SearchAggregateResults(searchResults);
    }

    public static SearchAggregateResults EmptyStub() => new([]);
}
