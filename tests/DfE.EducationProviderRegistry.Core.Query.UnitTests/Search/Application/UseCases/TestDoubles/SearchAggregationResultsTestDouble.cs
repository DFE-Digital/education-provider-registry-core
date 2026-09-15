using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchAggregationResultsTestDouble
{
    public static SearchProviderResults Stub()
    {
        List<SearchProviderResult> searchResults = [];

        for (int i = 0; i < new Bogus.Faker().Random.Int(1, 10); i++)
        {
            searchResults.Add(
                SearchResultTestDouble.Fake()); // Generate synthetic establishment search results instance
        }

        return new SearchProviderResults(searchResults);
    }

    public static SearchProviderResults EmptyStub() => new(null!);
}
