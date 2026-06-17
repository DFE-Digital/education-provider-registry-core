namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;

/// <summary>
/// Represents a strongly typed collection of <see cref="EstablishmentSearchResult"/> items
/// returned from an establishment search operation.
/// </summary>
public sealed class EstablishmentSearchResults
{
    private readonly List<EstablishmentSearchResult> _establishments;

    /// <summary>
    /// Gets the read-only collection of establishment search results.
    /// </summary>
    public IReadOnlyCollection<EstablishmentSearchResult> EstablishmentCollection => _establishments.AsReadOnly();

    /// <summary>
    /// Gets the number of establishment search results contained in the collection.
    /// Returns <c>0</c> if the underlying list is <c>null</c>.
    /// </summary>
    public int Count => _establishments?.Count ?? 0;

    /// <summary>
    /// Initializes a new, empty instance of the <see cref="EstablishmentSearchResults"/> class.
    /// </summary>
    public EstablishmentSearchResults()
    {
        _establishments = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentSearchResults"/> class
    /// using the provided collection of <see cref="EstablishmentSearchResult"/> items.
    /// </summary>
    /// <param name="establishments">
    /// The collection of establishment search results to populate the instance with.
    /// If <c>null</c>, an empty collection is used.
    /// </param>
    public EstablishmentSearchResults(IEnumerable<EstablishmentSearchResult> establishments)
    {
        _establishments = establishments?.ToList() ?? [];
    }

    /// <summary>
    /// Creates an empty <see cref="EstablishmentSearchResults"/> instance.
    /// </summary>
    /// <returns>
    /// A new <see cref="EstablishmentSearchResults"/> with no contained results.
    /// </returns>
    public static EstablishmentSearchResults CreateEmpty() => new();
}
