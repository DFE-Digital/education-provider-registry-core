using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;

internal sealed class SearchResultsFromContextMapper
    : IMapper<SearchPipelineContext, SearchResults<EstablishmentSearchResults, SearchFacets>>
{
    public SearchResults<EstablishmentSearchResults, SearchFacets> Map(SearchPipelineContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        EstablishmentSearchResult[] mapped =
            context.Get<EstablishmentSearchResult[]>()
            ?? throw new ArgumentNullException(nameof(context),
                "SearchPipelineContext does not contain EstablishmentSearchResult[].");

        List<SearchFacet> facets =
            context.Get<List<SearchFacet>>()
            ?? throw new ArgumentNullException(nameof(context),
                "SearchPipelineContext does not contain List<SearchFacet>.");

        EstablishmentSearchResults wrappedResults = new(mapped);
        SearchFacets wrappedFacets = new(facets);

        return new SearchResults<EstablishmentSearchResults, SearchFacets>
        {
            Results = wrappedResults,
            FacetResults = wrappedFacets
        };
    }
}
