using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchResultsTestDouble
{
    public static SearchResults<SearchProviderResults, SearchFacets> Stub() =>
        new()
        {
            Results = SearchAggregationResultsTestDouble.Stub(),    // Populated search aggregation results.
            FacetResults = SearchFacetsTestDouble.Stub()            // Populated facet results.
        };

    public static SearchResults<SearchProviderResults, SearchFacets> StubWithNoResults() =>
        new()
        {
            Results = SearchAggregationResultsTestDouble.EmptyStub(),   // Unpopulated search aggregation results.
            FacetResults = SearchFacetsTestDouble.Stub()                // Populated facet results.
        };
}
