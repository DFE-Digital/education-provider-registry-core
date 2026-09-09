namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Represents the classification or category of a search provider
/// (e.g., Academy, Free School, Local Authority Maintained).
/// This value object is used within search results to provide
/// a consistent, strongly typed representation of search provider type.
/// </summary>
public sealed record class SearchProviderType
{
    /// <summary>
    /// Gets the underlying establishment type name as returned
    /// by the search index or data source.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the underlying establishment type Id as returned
    /// by the search index or data source.
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchProviderType"/> record.
    /// </summary>
    /// <param name="name">
    /// The raw search provider type value. Must not be <c>null</c> or empty.
    /// </param>
    /// <param name="id">
    /// The raw search provider ID value.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="type"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="type"/> is empty or whitespace.
    /// </exception>
    public SearchProviderType(string type, long id)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(type);

        Name = type;
        Id = id;
    }

    /// <summary>
    /// Creates a new <see cref="SearchProviderType"/> instance.
    /// This factory method provides an intention‑revealing alternative
    /// to directly invoking the constructor.
    /// </summary>
    /// <param name="name">The raw search provider type value.</param>
    /// <param name="id">The raw search provider ID value.</param>
    /// <returns>
    /// A fully validated <see cref="SearchProviderType"/> instance.
    /// </returns>
    public static SearchProviderType Create(string name, long id) => new(name, id);
}
