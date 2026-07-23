using System.Collections.ObjectModel;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;

internal sealed class EstablishmentsSearchServiceAdapter
    : ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>
{
    private readonly ISearchProvider<Establishment> _idProvider;

    private readonly IEvaluator<SearchPipelineContext> _searchPipelineEvaluator;

    private readonly IMapper<
        SearchPipelineContext, SearchResults<
        EstablishmentSearchResults, SearchFacets>> _searchResultsFromContextMapper;

    private readonly IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>> _searchRequestFiltersToCoreFiltersMapper;

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

        ArgumentNullException.ThrowIfNull(facetProvider);
    }

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
