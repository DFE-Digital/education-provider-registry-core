using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchAggregationResultsTestDouble
{
    public static SearchAggregateResults Stub()
    {
        List<SearchAggregateResult> searchResults = [];

        for (int i = 0; i < new Bogus.Faker().Random.Int(1, 10); i++)
        {
            searchResults.Add(
                SearchResultTestDouble.Fake()); // Generate synthetic establishment search results instance
        }

        return new SearchAggregateResults(searchResults);
    }

    public static SearchAggregateResults EmptyStub() => new(null!);
}
