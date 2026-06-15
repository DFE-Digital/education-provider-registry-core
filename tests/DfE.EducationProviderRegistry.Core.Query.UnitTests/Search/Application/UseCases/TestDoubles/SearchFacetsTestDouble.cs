using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
public static class SearchFacetsTestDouble
{
    public static SearchFacets Stub()
    {
        List<SearchFacet> facets =
        [
            new SearchFacet("name", [new FacetResult("value1", 1)])
        ];

        return new SearchFacets(facets);
    }
}
