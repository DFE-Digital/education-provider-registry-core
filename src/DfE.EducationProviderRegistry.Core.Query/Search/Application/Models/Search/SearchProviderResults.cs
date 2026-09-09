namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Represents a strongly typed collection of <see cref="SearchProviderResult"/> items
/// returned from a provider search operation.
/// </summary>
public sealed class SearchProviderResults
{
    private readonly List<SearchProviderResult> _searchResult;

    /// <summary>
    /// Gets the read-only collection of search results.
    /// </summary>
    public IReadOnlyCollection<SearchProviderResult> SearchResultCollection => _searchResult.AsReadOnly();

    /// <summary>
    /// Gets the number of search results contained in the collection on this page.
    /// Returns <c>0</c> if the underlying list is <c>null</c>.
    /// </summary>
    public int Count => _searchResult?.Count ?? 0;

    /// <summary>
    /// Initializes a new, empty instance of the <see cref="SearchProviderResults"/> class.
    /// </summary>
    public SearchProviderResults()
    {
        _searchResult = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchProviderResults"/> class
    /// using the provided collection of <see cref="SearchProviderResult"/> items.
    /// </summary>
    /// <param name="establishments">
    /// The collection of search results to populate the instance with.
    /// If <c>null</c>, an empty collection is used.
    /// </param>
    public SearchProviderResults(IEnumerable<SearchProviderResult> searchResults)
    {
        _searchResult = searchResults?.ToList() ?? [];
    }

    /// <summary>
    /// Creates an empty <see cref="SearchProviderResults"/> instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="SearchProviderResults"/> with no contained results.
    /// </returns>
    public static SearchProviderResults CreateEmpty() => new();
}
