namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;

/// <summary>
/// Represents a single establishment returned as part of a search operation.
/// This model contains only presentation‑ready fields and is designed for
/// lightweight transport between the search layer and consuming clients.
/// </summary>
public sealed record EstablishmentSearchResult
{
    /// <summary>
    /// Gets the unique numeric identifier (URN) assigned to the establishment.
    /// </summary>
    public int Urn { get; }

    /// <summary>
    /// Gets the display name of the establishment as returned by the search index.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentSearchResult"/> record
    /// with the specified establishment identifier and display name.
    /// </summary>
    /// <param name="urn">
    /// The unique numeric identifier (URN) assigned to the establishment.
    /// </param>
    /// <param name="name">
    /// The display name of the establishment as returned by the search index.
    /// </param>
    public EstablishmentSearchResult(int urn, string name)
    {
        Urn = urn;
        Name = name;
    }
}
