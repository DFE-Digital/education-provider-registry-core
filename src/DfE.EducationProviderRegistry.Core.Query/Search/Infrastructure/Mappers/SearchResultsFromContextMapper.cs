using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;

/// <summary>
/// Maps a <see cref="SearchPipelineContext"/> into a strongly typed
/// <see cref="SearchResults{TResults, TFacets}"/> containing establishment
/// search results and facet metadata.
/// </summary>
internal sealed class SearchResultsFromContextMapper
    : IMapper<SearchPipelineContext, SearchResults<EstablishmentSearchResults, SearchFacets>>
{
    /// <summary>
    /// Extracts mapped establishment results and facet results from the pipeline
    /// context and wraps them in a <see cref="SearchResults{TResults, TFacets}"/>.
    /// </summary>
    /// <param name="context">The pipeline context containing search output.</param>
    /// <returns>A populated <see cref="SearchResults{TResults, TFacets}"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the context or required components are missing.
    /// </exception>
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
