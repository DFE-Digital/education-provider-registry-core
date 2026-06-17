using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;

/// <summary>
/// Represents a fully validated and immutable request object used by the
/// <see cref="ISearchServiceAdapter{TSearchResult, TFacetResult}"/> to execute
/// a search operation against an underlying search provider.
/// </summary>
public sealed class SearchServiceAdapterRequest
{
    /// <summary>
    /// Gets the key used to identify and resolve the search index
    /// that the underlying search provider should query.
    /// </summary>
    public string SearchIndexKey { get; }

    /// <summary>
    /// Gets the keyword or phrase used to perform the search.
    /// </summary>
    public string SearchKeyword { get; }

    /// <summary>
    /// Gets the number of search results to skip. Defaults to zero,
    /// indicating that no records will be omitted from the beginning
    /// of the result set.
    /// </summary>
    public int Offset { get; } = 0;

    /// <summary>
    /// Gets the specific fields within the underlying data source
    /// that should be queried for keyword matching.
    /// </summary>
    public IList<string> SearchFields { get; }

    /// <summary>
    /// Gets the set of facet fields to apply to the search query
    /// for grouped filtering or aggregation.
    /// </summary>
    public IList<string> Facets { get; }

    /// <summary>
    /// Gets the optional list of filter conditions, where each entry
    /// maps a filter name to a set of allowed values.
    /// </summary>
    public IList<FilterRequest> SearchFilterRequests { get; }

    /// <summary>
    /// Gets the configured <see cref="SortOrder"/> instance representing
    /// the field and direction used to order search results.
    /// </summary>
    public SortOrder SortOrdering { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchServiceAdapterRequest"/> class,
    /// ensuring all required fields are populated and immutable for consistent query execution.
    /// </summary>
    /// <param name="searchIndexKey">
    /// The key used to resolve the search index to query.
    /// </param>
    /// <param name="searchKeyword">
    /// The keyword or phrase to match against the configured search fields.
    /// </param>
    /// <param name="searchFields">
    /// The list of fields to include in the keyword-based search operation.
    /// </param>
    /// <param name="sortOrdering">
    /// The ordering configuration specifying the field and direction to sort results by.
    /// </param>
    /// <param name="facets">
    /// Optional list of facet keys for grouping and filtering results.
    /// </param>
    /// <param name="searchFilterRequests">
    /// Optional filters for refining the search query, keyed by filter name.
    /// </param>
    /// <param name="offset">
    /// Specifies how many results to skip in the returned dataset. Defaults to zero.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="searchIndexKey"/> or <paramref name="searchKeyword"/> is null or whitespace.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="searchFields"/> is null or empty.
    /// </exception>
    public SearchServiceAdapterRequest(
        string searchIndexKey,
        string searchKeyword,
        IList<string> searchFields,
        SortOrder sortOrdering,
        IList<string>? facets = null,
        IList<FilterRequest>? searchFilterRequests = null,
        int offset = 0)
    {
        SearchIndexKey = !string.IsNullOrWhiteSpace(searchIndexKey) ?
            searchIndexKey :
            throw new ArgumentException($"{nameof(searchIndexKey)} cannot be null or whitespace.", searchIndexKey);

        SearchKeyword = !string.IsNullOrWhiteSpace(searchKeyword)
            ? searchKeyword
            : throw new ArgumentException(
                $"{nameof(searchKeyword)} cannot be null or whitespace.", nameof(searchKeyword));

        SearchFields = searchFields?.Count > 0
            ? searchFields
            : throw new ArgumentException(
                $"A valid {nameof(searchFields)} argument must be provided.", nameof(searchFields));

        SortOrdering = sortOrdering;
        Facets = facets ?? [];
        SearchFilterRequests = searchFilterRequests ?? [];
        Offset = offset;
    }

    /// <summary>
    /// Creates a new <see cref="SearchServiceAdapterRequest"/> instance without
    /// requiring direct use of the constructor. Improves readability and
    /// usability for consumers of the API.
    /// </summary>
    /// <param name="searchIndexKey">The key used to resolve the search index.</param>
    /// <param name="searchKeyword">The keyword or phrase to search for.</param>
    /// <param name="searchFields">The fields to include in the search.</param>
    /// <param name="facets">The facet fields to apply to the search.</param>
    /// <param name="sortOrdering">The ordering configuration for the results.</param>
    /// <param name="searchFilterRequests">Optional filter conditions.</param>
    /// <param name="offset">The number of results to skip.</param>
    /// <returns>
    /// A fully constructed <see cref="SearchServiceAdapterRequest"/> instance.
    /// </returns>
    public static SearchServiceAdapterRequest Create(
        string searchIndexKey,
        string searchKeyword,
        IList<string> searchFields,
        IList<string> facets,
        SortOrder sortOrdering,
        IList<FilterRequest>? searchFilterRequests = null,
        int offset = 0)
            => new(searchIndexKey, searchKeyword, searchFields, sortOrdering, facets, searchFilterRequests, offset);
}
