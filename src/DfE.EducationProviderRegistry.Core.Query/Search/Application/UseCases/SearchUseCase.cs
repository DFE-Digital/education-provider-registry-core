using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using Microsoft.Extensions.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases;

/// <summary>
/// Executes an establishment search operation using the configured
/// <see cref="ISearchServiceAdapter{TSearchResult, TFacetResult}"/> and returns
/// the results wrapped in a <see cref="UseCaseResponse{T}"/>.
/// </summary>
public sealed class SearchUseCase : IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>
{
    private readonly ILogger<SearchUseCase> _logger;
    private readonly SearchCriteria _searchCriteria;
    private readonly ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets> _searchServiceAdapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchUseCase"/> class.
    /// </summary>
    /// <param name="logger">The logger used for structured diagnostic logging.</param>
    /// <param name="searchCriteria">
    /// The configured search criteria defining allowed fields, facets, and sort options.
    /// </param>
    /// <param name="searchServiceAdapter">
    /// The adapter responsible for executing the search against the underlying search provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any required dependency is null.
    /// </exception>
    public SearchUseCase(
        ILogger<SearchUseCase> logger,
        SearchCriteria searchCriteria,
        ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets> searchServiceAdapter)
    {
        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));
        _searchCriteria = searchCriteria ??
            throw new ArgumentNullException(nameof(searchCriteria));
        _searchServiceAdapter = searchServiceAdapter ??
            throw new ArgumentNullException(nameof(searchServiceAdapter));
    }

    /// <summary>
    /// Handles the establishment search request and returns the results wrapped in a
    /// <see cref="UseCaseResponse{T}"/>.
    /// </summary>
    /// <param name="request">The validated search request containing query parameters.</param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="UseCaseResponse{T}"/> containing the search results,
    /// or an error response if the operation fails or is cancelled.
    /// </returns>
    public async Task<UseCaseResponse<SearchResponse>> HandleRequestAsync(
        SearchRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            SearchResults<EstablishmentSearchResults, SearchFacets>? searchResults =
                await _searchServiceAdapter.SearchAsync(
                    SearchServiceAdapterRequest.Create(
                        request.SearchIndexKey,
                        request.SearchKeywords,
                        _searchCriteria.SearchFields,
                        _searchCriteria.Facets,
                        request.SortOrder,
                        request.FilterRequests,
                        request.Offset),
                    cancellationToken);

            SearchResponse model = new(searchResults.Results!, searchResults.FacetResults!);

            return UseCaseResponse<SearchResponse>.Success(model);
        }
        catch (OperationCanceledException ex)
        {
            const string message = "The search request was cancelled by the caller.";

            _logger.LogWarning(
                ex,
                "{UseCase} execution cancelled: {Message}",
                nameof(SearchUseCase),
                message);

            return UseCaseResponse<SearchResponse>.Failure(message);
        }
        catch (SearchException ex)
        {
            const string message = "A domain-specific error occurred during search.";

            _logger.LogError(
                ex,
                "{UseCase} domain-specific error: {Message}",
                nameof(SearchUseCase),
                message);

            return UseCaseResponse<SearchResponse>.Failure(message);
        }
        catch (Exception ex)
        {
            const string message = "An unexpected error occurred while processing the search request.";

            _logger.LogError(
                ex,
                "{UseCase} unexpected error: {Message}",
                nameof(SearchUseCase),
                message);

            return UseCaseResponse<SearchResponse>.Failure(message);
        }
    }
}
