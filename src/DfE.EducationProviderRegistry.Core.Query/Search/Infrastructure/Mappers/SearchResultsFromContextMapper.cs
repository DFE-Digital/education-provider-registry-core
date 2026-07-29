using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;

internal sealed class SearchResultsFromContextMapper
    : IMapper<(IReadOnlyList<EstablishmentReadModel> Results, IReadOnlyList<AggregatedFacetResult> Facets),
              SearchResults<EstablishmentSearchResults, SearchFacets>>
{
    public SearchResults<EstablishmentSearchResults, SearchFacets> Map(
        (IReadOnlyList<EstablishmentReadModel> Results, IReadOnlyList<AggregatedFacetResult> Facets) context)
    {
        if (context.Results is null)
        {
            throw new ArgumentNullException(nameof(context),
                "Tuple does not contain establishment results.");
        }

        if (context.Facets is null)
        {
            throw new ArgumentNullException(nameof(context),
                "Tuple does not contain facet results.");
        }

        // Convert EstablishmentReadModel → EstablishmentSearchResult
        EstablishmentSearchResult[] mapped =
            [.. context.Results
                   .Select(r => EstablishmentSearchResult.Create(
                       new Shared.UniqueReferenceNumber(r.Urn),
                       new Shared.Name(r.Name),
                       new Shared.Address(
                           Street: "",
                           Town: r.City ?? "",
                           County: "",
                           Postcode: r.Postcode ?? ""),
                       new EstablishmentType(r.Type),
                       new GroupDetail("X", "y"),
                       new LocalAuthority("LA_name", "LA_Code")))];

        // Convert FacetResultNew → SearchFacet (correct mapping)
        List<SearchFacet> facets =
            [.. context.Facets
                   .Select(facetResult =>
                       new SearchFacet(
                           facetResult.FacetName,
                           [.. facetResult.Values.Select(facetValue => new FacetResult(facetValue.Value, facetValue.Count))]
                       )
                   )];

        return new SearchResults<EstablishmentSearchResults, SearchFacets>
        {
            Results = new EstablishmentSearchResults(mapped),
            FacetResults = new SearchFacets(facets)
        };
    }
}
