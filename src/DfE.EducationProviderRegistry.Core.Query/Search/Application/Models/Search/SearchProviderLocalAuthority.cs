namespace DfE.EducationProviderRegistry.Core.Query.Shared;

/// <summary>
/// Represents the local authority associated with an establishment.
/// This value object provides a strongly typed representation of both
/// the authority's display name.
/// </summary>
public sealed record class SearchProviderLocalAuthority
{
    /// <summary>
    /// Gets the display name of the local authority.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchProviderLocalAuthority"/> record.
    /// </summary>
    /// <param name="localAuthorityName">The display name of the local authority.</param>
    public SearchProviderLocalAuthority(string localAuthorityName)
    {
        Name = localAuthorityName;
    }

    /// <summary>
    /// Creates a new <see cref="SearchProviderLocalAuthority"/> instance.
    /// This factory method provides an intention‑revealing alternative
    /// to directly invoking the constructor.
    /// </summary>
    /// <param name="localAuthorityName">The display name of the local authority.</param>
    /// <returns>A fully validated <see cref="SearchProviderLocalAuthority"/> instance.</returns>
    public static SearchProviderLocalAuthority Create(string localAuthorityName) => new(localAuthorityName);
}
