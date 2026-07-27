using System.Collections.ObjectModel;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.Shared.Pipeline;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;

/// <summary>
/// Adapts search requests for <see cref="Establishment"/> entities into the
/// trigram‑based search pipeline, executes the pipeline, and maps the results
/// into <see cref="SearchResults{TResults, TFacets}"/>.
/// </summary>
internal sealed class EstablishmentsSearchServiceAdapter
    : ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>
{
    private readonly ISearchProvider<Establishment> _idProvider;

    private readonly IEvaluator<SearchPipelineContext> _searchPipelineEvaluator;

    private readonly IMapper<
        SearchPipelineContext,
        SearchResults<EstablishmentSearchResults, SearchFacets>> _searchResultsFromContextMapper;

    private readonly IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>> _searchRequestFiltersToCoreFiltersMapper;

    /// <summary>
    /// Creates a new search service adapter for establishments.
    /// </summary>
    /// <param name="idProvider">Provides trigram‑based search over establishments.</param>
    /// <param name="facetProvider">Ensures facet provider is available for pipeline steps.</param>
    /// <param name="pipeline">The ordered pipeline steps to execute.</param>
    /// <param name="searchResultsFromContextMapper">Maps pipeline context to search results.</param>
    /// <param name="searchRequestFiltersToCoreFiltersMapper">Maps API filter requests to core filter requests.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any required dependency is <c>null</c>.
    /// </exception>
    public EstablishmentsSearchServiceAdapter(
        ISearchProvider<Establishment> idProvider,
        IFacetProvider facetProvider,
        IEvaluator<SearchPipelineContext> evaluator,
        IMapper<
            SearchPipelineContext,
            SearchResults<EstablishmentSearchResults, SearchFacets>> searchResultsFromContextMapper,
        IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>> searchRequestFiltersToCoreFiltersMapper)
    {
        _idProvider = idProvider ??
            throw new ArgumentNullException(nameof(idProvider));
        _searchResultsFromContextMapper = searchResultsFromContextMapper ??
            throw new ArgumentNullException(nameof(searchResultsFromContextMapper));
        _searchRequestFiltersToCoreFiltersMapper = searchRequestFiltersToCoreFiltersMapper ??
            throw new ArgumentNullException(nameof(searchRequestFiltersToCoreFiltersMapper));

        ArgumentNullException.ThrowIfNull(evaluator);
        _searchPipelineEvaluator = evaluator;

        ArgumentNullException.ThrowIfNull(facetProvider);
    }

    /// <summary>
    /// Executes a trigram search for establishments, applies pipeline steps,
    /// and maps the results into <see cref="SearchResults{TResults, TFacets}"/>.
    /// </summary>
    /// <param name="request">The search request containing keyword, filters, and paging.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// A <see cref="SearchResults{TResults, TFacets}"/> instance containing
    /// establishment search results and facet information.
    /// </returns>
    public async Task<SearchResults<EstablishmentSearchResults, SearchFacets>> SearchAsync(
        SearchServiceAdapterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        IReadOnlyList<Establishment> establishments =
            await _idProvider.GetMatchingIdsAsync(
                searchTerm: request.SearchKeyword,
                pageSize: 50,
                offset: request.Offset,
                filters: _searchRequestFiltersToCoreFiltersMapper
                    .Map(request.SearchFilterRequests.AsReadOnly()),
                cancellationToken);

        ReadOnlyCollection<string?> availableEstablishmentIids =
            establishments.Select(establishment =>
                establishment.Urn).ToList().AsReadOnly();

        if (availableEstablishmentIids.Count == 0)
        {
            return new SearchResults<EstablishmentSearchResults, SearchFacets>();
        }

        SearchPipelineContext context = new();
        context.Set(availableEstablishmentIids);
        context.Set(establishments);
        context.Set(new List<string> { "EstablishmentTypeId" });

        await _searchPipelineEvaluator.EvaluateAsync(context, cancellationToken);

        return _searchResultsFromContextMapper.Map(context);
    }
}
