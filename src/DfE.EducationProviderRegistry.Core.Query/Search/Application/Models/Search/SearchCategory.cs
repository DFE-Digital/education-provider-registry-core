namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Represents the classification or category of a search provider
/// (e.g. 'Group' OR 'Establishment').
/// This value object is used within search results to provide
/// a consistent, strongly typed representation of search provider catgeory.
/// </summary>
public sealed record class SearchCategory
{
    /// <summary>
    /// Gets the underlying search provider category value as returned
    /// by the search index or data source.
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchCategory"/> record.
    /// </summary>
    /// <param name="type">
    /// The raw search provider category value. Must not be <c>null</c> or empty.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="category"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="category"/> is empty or whitespace.
    /// </exception>
    public SearchCategory(string category)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(category);

        Category = category;
    }

    /// <summary>
    /// Creates a new <see cref="SearchCategory"/> instance.
    /// This factory method provides an intention‑revealing alternative
    /// to directly invoking the constructor.
    /// </summary>
    /// <param name="category">The raw search provider category value.</param>
    /// <returns>
    /// A fully validated <see cref="SearchCategory"/> instance.
    /// </returns>
    public static SearchCategory Create(string category) => new(category);
}
