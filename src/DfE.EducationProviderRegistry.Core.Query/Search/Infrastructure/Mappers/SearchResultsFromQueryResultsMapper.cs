using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;

internal sealed class SearchResultsFromQueryResultsMapper
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

        EstablishmentSearchResult[] mapped = null!;

        mapped =
        [
            .. context.Results.Select(r =>
                EstablishmentSearchResult.Create(
                    new UniqueReferenceNumber(r.Urn),
                    new Name(r.Name ?? string.Empty),
                    new SiteAddressModel(
                        Name: r.Name ?? string.Empty,
                        AddressLine1: r.AddressLine1 ?? string.Empty,
                        AddressLine2: r.AddressLine2 ?? string.Empty,
                        Town: r.City ?? string.Empty,
                        County: r.County ?? string.Empty,
                        Postcode: r.Postcode ?? string.Empty),
                    new EstablishmentType(r.Type ?? string.Empty),
                    new GroupDetail(
                        partOfName: r.GroupName ?? string.Empty,
                        partOfCode: r.GroupCode ?? string.Empty),
                    new LocalAuthority(
                        localAuthorityName: r.LocalAuthorityName ?? string.Empty,
                        localAuthorityCode: r.LocalAuthorityCode ?? string.Empty)
                )
        )];

        List<SearchFacet> facets =
            [.. context.Facets
                   .Select(facetResult =>
                       new SearchFacet(
                           facetResult.FacetName,
                           [.. facetResult.Values.Select(facetValue => new FacetResult(facetValue.Value, facetValue.Label, facetValue.Count))]
                       )
                   )];

        return new SearchResults<EstablishmentSearchResults, SearchFacets>
        {
            Results = new EstablishmentSearchResults(mapped),
            FacetResults = new SearchFacets(facets)
        };
    }
}
