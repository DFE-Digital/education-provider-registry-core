using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;

//[ExcludeFromCodeCoverage]
//public static class SearchResponseTestDouble
//{
//    public static SearchResponse Stub()
//    {
//        EstablishmentSearchResults results = EstablishmentSearchResultsTestDouble.Stub();
//        SearchFacets facets = SearchFacetsTestDouble.Stub();
//        return new(results, facets, results.Count);
//    }

//    public static SearchResults<EstablishmentSearchResults, SearchFacets> StubWithNoResults() =>
//        new()
//        {
//            Results = EstablishmentSearchResultsTestDouble.EmptyStub(),
//            FacetResults = SearchFacetsTestDouble.StubEmpty()
//        };
//}
