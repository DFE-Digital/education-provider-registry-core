namespace DfE.EducationProviderRegistry.Core.Query.Shared;

/// <summary>
/// Represents the local authority associated with an establishment.
/// This value object provides a strongly typed representation of both
/// the authority's display name and its identifying code.
/// </summary>
public sealed record class LocalAuthority
{
    /// <summary>
    /// Gets the display name of the local authority.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the identifying code of the local authority.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalAuthority"/> record.
    /// </summary>
    /// <param name="localAuthorityName">The display name of the local authority.</param>
    /// <param name="localAuthorityCode">The identifying code of the local authority.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="localAuthorityName"/> or <paramref name="localAuthorityCode"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="localAuthorityName"/> or <paramref name="localAuthorityCode"/> is empty or whitespace.
    /// </exception>
    public LocalAuthority(string localAuthorityName, string localAuthorityCode)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(localAuthorityCode);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(localAuthorityName);

        Name = localAuthorityName;
        Code = localAuthorityCode;
    }

    /// <summary>
    /// Creates a new <see cref="LocalAuthority"/> instance.
    /// This factory method provides an intention‑revealing alternative
    /// to directly invoking the constructor.
    /// </summary>
    /// <param name="localAuthorityName">The display name of the local authority.</param>
    /// <param name="localAuthorityCode">The identifying code of the local authority.</param>
    /// <returns>A fully validated <see cref="LocalAuthority"/> instance.</returns>
    public static LocalAuthority Create(
        string localAuthorityName,
        string localAuthorityCode) =>
            new(localAuthorityName, localAuthorityCode);
}
