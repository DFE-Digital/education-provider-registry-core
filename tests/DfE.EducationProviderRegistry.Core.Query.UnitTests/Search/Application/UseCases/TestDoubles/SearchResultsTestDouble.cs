using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchResultsTestDouble
{
    public static SearchResults<EstablishmentSearchResults, SearchFacets> Stub() =>
        new()
        {
            Results = EstablishmentSearchResultsTestDouble.Stub(),  // Populated establishment search results
            FacetResults = SearchFacetsTestDouble.Stub()            // Populated facet results
        };

    public static SearchResults<EstablishmentSearchResults, SearchFacets> StubWithNoResults() =>
        new()
        {
            Results = EstablishmentSearchResultsTestDouble.EmptyStub(), // Unpopulated establishment search results
            FacetResults = SearchFacetsTestDouble.Stub()                // Populated facet results
        };
}
