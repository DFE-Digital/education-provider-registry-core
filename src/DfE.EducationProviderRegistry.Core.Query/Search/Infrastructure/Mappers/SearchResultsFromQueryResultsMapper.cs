using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Facets;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;

internal sealed class SearchResultsFromQueryResultsMapper
    : IMapper<
        (
            IReadOnlyList<SearchReadModel> Results,
            IReadOnlyList<AggregatedFacetResult> Facets,
            int TotalCount
        ),
        SearchResults<SearchProviderResults, SearchFacets>>
{
    public SearchResults<SearchProviderResults, SearchFacets> Map(
        (
            IReadOnlyList<SearchReadModel> Results,
            IReadOnlyList<AggregatedFacetResult> Facets,
            int TotalCount
        ) context)
    {
        if (context.Results is null)
        {
            throw new ArgumentNullException(
                nameof(context),
                "Tuple does not contain search provider results.");
        }

        if (context.Facets is null)
        {
            throw new ArgumentNullException(
                nameof(context),
                "Tuple does not contain facet results.");
        }

        SearchProviderResult[] mapped =
        [
            .. context.Results.Select(r =>
                SearchProviderResult.Create(
                    new ProviderIdentifier(r.Id),
                    new Name(r.Name ?? string.Empty),
                    new SearchProviderAddress(
                        FullAddress: r.Address ?? string.Empty),
                    new SearchProviderType(r.TypeName ?? string.Empty, r.TypeId),
                    new GroupDetail(
                        partOfName: r.GroupName ?? string.Empty,
                        partOfCode: r.GroupCode ?? string.Empty),
                    new SearchProviderLocalAuthority(
                        localAuthorityName: r.LocalAuthorityName ?? string.Empty),
                    new SearchProviderCategory(r.ProviderCategory)
                )
            )
        ];

        List<SearchFacet> facets =
        [
            .. context.Facets.Select(facetResult =>
                new SearchFacet(
                    facetResult.FacetName,
                    [
                        .. facetResult.Values.Select(facetValue =>
                            new FacetResult(
                                facetValue.Value,
                                facetValue.Label,
                                facetValue.Count))
                    ]))
        ];

        return new SearchResults<SearchProviderResults, SearchFacets>
        {
            Results = new SearchProviderResults(mapped),
            FacetResults = new SearchFacets(facets),
            TotalCount = context.TotalCount
        };
    }
}
