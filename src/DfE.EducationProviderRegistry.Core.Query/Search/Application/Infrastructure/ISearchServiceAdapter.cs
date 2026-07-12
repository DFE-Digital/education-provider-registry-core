using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;

/// <summary>
/// Defines an adapter responsible for executing search operations against an
/// underlying search provider and returning strongly typed results.
/// </summary>
/// <typeparam name="TSearchResult">
/// The type representing the primary search results returned by the provider.
/// </typeparam>
/// <typeparam name="TFacetResult">
/// The type representing the faceted aggregation results used for filtering,
/// analytics, or navigation.
/// </typeparam>
public interface ISearchServiceAdapter<TSearchResult, TFacetResult>
{
    /// <summary>
    /// Executes a search operation using the supplied request parameters and returns
    /// both the primary results and associated facet data.
    /// </summary>
    /// <param name="request">
    /// The request containing the validated search parameters to be sent to the
    /// underlying search provider.
    /// </param>
    /// <param name="cancellationToken">
    /// A token that may be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    /// A <see cref="SearchResults{TSearchResult, TFacetResult}"/> instance containing
    /// the matched results and any faceted aggregation data returned by the provider.
    /// </returns>
    Task<SearchResults<TSearchResult, TFacetResult>> SearchAsync(
        SearchServiceAdapterRequest request,
        CancellationToken cancellationToken = default);
}
