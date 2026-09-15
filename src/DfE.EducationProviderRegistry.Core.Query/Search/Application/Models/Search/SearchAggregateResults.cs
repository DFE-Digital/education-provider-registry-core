namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Represents a strongly typed collection of <see cref="SearchAggregateResult"/> items
/// returned from a provider search operation.
/// </summary>
public sealed class SearchAggregateResults
{
    private readonly List<SearchAggregateResult> _searchResult;

    /// <summary>
    /// Gets the read-only collection of search results.
    /// </summary>
    public IReadOnlyCollection<SearchAggregateResult> SearchResultCollection => _searchResult.AsReadOnly();

    /// <summary>
    /// Gets the number of search results contained in the collection on this page.
    /// Returns <c>0</c> if the underlying list is <c>null</c>.
    /// </summary>
    public int Count => _searchResult?.Count ?? 0;

    /// <summary>
    /// Initializes a new, empty instance of the <see cref="SearchAggregateResults"/> class.
    /// </summary>
    public SearchAggregateResults()
    {
        _searchResult = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchAggregateResults"/> class
    /// using the provided collection of <see cref="SearchAggregateResult"/> items.
    /// </summary>
    /// <param name="searchResults">
    /// The collection of search results to populate the instance with.
    /// If <c>null</c>, an empty collection is used.
    /// </param>
    public SearchAggregateResults(IEnumerable<SearchAggregateResult> searchResults)
    {
        _searchResult = searchResults?.ToList() ?? [];
    }

    /// <summary>
    /// Creates an empty <see cref="SearchAggregateResults"/> instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="SearchAggregateResults"/> with no contained results.
    /// </returns>
    public static SearchAggregateResults CreateEmpty() => new();
}
