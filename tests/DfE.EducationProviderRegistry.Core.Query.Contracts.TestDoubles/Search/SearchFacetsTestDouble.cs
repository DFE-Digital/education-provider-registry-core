using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;

public static class SearchFacetsTestDouble
{
    public static SearchFacets Stub()
    {
        return new(
        [
            CreateFacet(
                name: "facet1",
                facets: [
                    new FacetResult(value: "value-1", label: "label-1", 5),
                    new FacetResult(value: "value-2", label: "label-2", 7)
                ]),
            CreateFacet(
                name: "facet2",
                facets: [
                    new FacetResult(value: "value-3", label: "label-3", 2),
                    new FacetResult(value: "value-4", label: "label-4", 8)
                ]),
        ]);
    }

    public static SearchFacets StubEmpty() => new([]);

    public static SearchFacets Create(params SearchFacet[] facets) => new(facets.ToList());

    private static SearchFacet CreateFacet(string name, FacetResult[] facets)
    {
        return new(name, facets);
    }
}
