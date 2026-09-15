using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchResultsTestDouble
{
    public static SearchResults<SearchAggregateResults, SearchFacets> Stub()
    {
        SearchAggregateResults results = SearchAggregationResultsTestDouble.Stub();

        return new SearchResults<SearchAggregateResults, SearchFacets>
        {
            Results = results,
            FacetResults = SearchFacetsTestDouble.Stub(),
            TotalCount = results.Count
        };
    }

    public static SearchResults<SearchAggregateResults, SearchFacets> StubWithNoResults() =>
        new()
        {
            Results = SearchAggregationResultsTestDouble.EmptyStub(),   // Unpopulated search aggregation results.
            FacetResults = SearchFacetsTestDouble.Stub(),               // Populated facet results.
            TotalCount = 0
        };
}
