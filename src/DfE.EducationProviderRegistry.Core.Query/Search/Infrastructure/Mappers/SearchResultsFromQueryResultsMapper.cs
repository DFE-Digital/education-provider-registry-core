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
        SearchResults<SearchAggregateResults, SearchFacets>>
{
    public SearchResults<SearchAggregateResults, SearchFacets> Map(
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

        SearchAggregateResult[] mapped =
        [
            .. context.Results.Select(r =>
                SearchAggregateResult.Create(
                    new ProviderIdentifier(r.Id),
                    new Name(r.Name ?? string.Empty),
                    new SearchAddress(
                        FullAddress: r.Address ?? string.Empty),
                    new SearchType(r.TypeName ?? string.Empty, r.TypeId),
                    new GroupDetail(
                        partOfName: r.GroupName ?? string.Empty,
                        partOfCode: r.GroupCode ?? string.Empty),
                    new SearchLocalAuthority(
                        localAuthorityName: r.LocalAuthorityName ?? string.Empty),
                    new SearchCategory(r.ProviderCategory),
                    r.AcademyCount
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

        return new SearchResults<SearchAggregateResults, SearchFacets>
        {
            Results = new SearchAggregateResults(mapped),
            FacetResults = new SearchFacets(facets),
            TotalCount = context.TotalCount
        };
    }
}
