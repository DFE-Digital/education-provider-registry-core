using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;

[ExcludeFromCodeCoverage]
public static class SearchResultsTestDouble
{
    public static SearchResults<SearchAggregateResults, SearchFacets> Stub()
    {
        SearchAggregateResults results = SearchAggregateResultsTestDouble.Stub();

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
            Results = SearchAggregateResultsTestDouble.EmptyStub(),   // Unpopulated search aggregation results.
            FacetResults = SearchFacetsTestDouble.Stub(),               // Populated facet results.
            TotalCount = 0
        };
}
