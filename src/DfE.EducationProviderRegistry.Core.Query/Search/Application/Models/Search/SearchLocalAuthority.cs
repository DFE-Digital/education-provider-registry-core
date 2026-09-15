namespace DfE.EducationProviderRegistry.Core.Query.Shared;

/// <summary>
/// Represents the local authority associated with a search result.
/// This value object provides a strongly typed representation of both
/// the authority's display name.
/// </summary>
public sealed record class SearchLocalAuthority
{
    /// <summary>
    /// Gets the display name of the local authority.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchLocalAuthority"/> record.
    /// </summary>
    /// <param name="localAuthorityName">The display name of the local authority.</param>
    public SearchLocalAuthority(string localAuthorityName)
    {
        Name = localAuthorityName;
    }

    /// <summary>
    /// Creates a new <see cref="SearchLocalAuthority"/> instance.
    /// This factory method provides an intention‑revealing alternative
    /// to directly invoking the constructor.
    /// </summary>
    /// <param name="localAuthorityName">The display name of the local authority.</param>
    /// <returns>A fully validated <see cref="SearchLocalAuthority"/> instance.</returns>
    public static SearchLocalAuthority Create(string localAuthorityName) => new(localAuthorityName);
}
