using System.Collections.ObjectModel;
using DfE.Core.Libraries.CrossCutting.Mapper;
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
    private readonly ISearchProvider<Establishment> _establishmentSearchProvider;

    private readonly IReadOnlyList<ISearchPipelineStep> _pipeline;

    private readonly IMapper<
        SearchPipelineContext, SearchResults<
        EstablishmentSearchResults, SearchFacets>> _searchResultsFromContextMapper;

    private readonly IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>> _searchRequestFiltersToCoreFiltersMapper;

    public EstablishmentsSearchServiceAdapter(
        ISearchProvider<Establishment> establishmentSearchProvider,
        IFacetProvider facetProvider,
        IEnumerable<ISearchPipelineStep> pipeline,
        IMapper<
            SearchPipelineContext,
            SearchResults<EstablishmentSearchResults, SearchFacets>> searchResultsFromContextMapper,
        IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>> searchRequestFiltersToCoreFiltersMapper)
    {
        _establishmentSearchProvider = establishmentSearchProvider ??
            throw new ArgumentNullException(nameof(establishmentSearchProvider));
        _searchResultsFromContextMapper = searchResultsFromContextMapper ??
            throw new ArgumentNullException(nameof(searchResultsFromContextMapper));
        _searchRequestFiltersToCoreFiltersMapper = searchRequestFiltersToCoreFiltersMapper ??
            throw new ArgumentNullException(nameof(searchRequestFiltersToCoreFiltersMapper));

        ArgumentNullException.ThrowIfNull(pipeline);
        _pipeline = pipeline.ToList().AsReadOnly();

        ArgumentNullException.ThrowIfNull(facetProvider);
    }

    public async Task<SearchResults<EstablishmentSearchResults, SearchFacets>> SearchAsync(
        SearchServiceAdapterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        IReadOnlyList<Establishment> establishments =
            await _establishmentSearchProvider.GetMatchingIdsAsync(
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

        foreach (ISearchPipelineStep step in _pipeline)
        {
            step.Execute(context, cancellationToken);
        }

        return _searchResultsFromContextMapper.Map(context);
    }
}
