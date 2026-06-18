namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;

/// <summary>
/// Represents the classification or category of an establishment
/// (e.g., Academy, Free School, Local Authority Maintained).
/// This value object is used within search results to provide
/// a consistent, strongly typed representation of establishment type.
/// </summary>
public sealed record class EstablishmentType
{
    /// <summary>
    /// Gets the underlying establishment type value as returned
    /// by the search index or data source.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentType"/> record.
    /// </summary>
    /// <param name="type">
    /// The raw establishment type value. Must not be <c>null</c> or empty.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="type"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="type"/> is empty or whitespace.
    /// </exception>
    public EstablishmentType(string type)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(type);

        Value = type;
    }

    /// <summary>
    /// Creates a new <see cref="EstablishmentType"/> instance.
    /// This factory method provides an intention‑revealing alternative
    /// to directly invoking the constructor.
    /// </summary>
    /// <param name="type">The raw establishment type value.</param>
    /// <returns>
    /// A fully validated <see cref="EstablishmentType"/> instance.
    /// </returns>
    public static EstablishmentType Create(string type) => new(type);
}
