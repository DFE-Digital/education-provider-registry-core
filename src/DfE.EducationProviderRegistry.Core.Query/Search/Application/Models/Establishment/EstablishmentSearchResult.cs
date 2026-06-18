using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;

/// <summary>
/// Represents a single establishment returned as part of a search operation.
/// This model contains presentation‑ready fields optimised for lightweight
/// transport between the search layer and consuming clients.
/// </summary>
public sealed record EstablishmentSearchResult
{
    /// <summary>
    /// Gets the unique numeric identifier (URN) assigned to the establishment.
    /// </summary>
    public UniqueReferenceNumber Urn { get; }

    /// <summary>
    /// Gets the display name of the establishment.
    /// </summary>
    public Name Name { get; }

    /// <summary>
    /// Gets the postal address associated with the establishment.
    /// </summary>
    public Address Address { get; }

    /// <summary>
    /// Gets the establishment type classification (e.g., Academy, Free School).
    /// </summary>
    public EstablishmentType Type { get; }

    /// <summary>
    /// Gets the group‑level details associated with the establishment,
    /// such as trust or federation information.
    /// </summary>
    public GroupDetail Group { get; }

    /// <summary>
    /// Gets the local authority responsible for the establishment.
    /// </summary>
    public LocalAuthority LocalAuthority { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentSearchResult"/> record
    /// using the specified establishment attributes.
    /// </summary>
    /// <param name="urn">The unique numeric identifier assigned to the establishment.</param>
    /// <param name="name">The display name of the establishment.</param>
    /// <param name="address">The postal address of the establishment.</param>
    /// <param name="type">The establishment type classification.</param>
    /// <param name="group">The group‑level details associated with the establishment.</param>
    /// <param name="localAuthority">The local authority responsible for the establishment.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any required parameter is <c>null</c>.
    /// </exception>
    public EstablishmentSearchResult(
        UniqueReferenceNumber urn,
        Name name,
        Address address,
        EstablishmentType type,
        GroupDetail group,
        LocalAuthority localAuthority)
    {
        Urn = urn ?? throw new ArgumentNullException(nameof(urn));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Group = group ?? throw new ArgumentNullException(nameof(group));
        LocalAuthority = localAuthority ?? throw new ArgumentNullException(nameof(localAuthority));
    }

    /// <summary>
    /// Creates a new <see cref="EstablishmentSearchResult"/> instance using the
    /// supplied establishment attributes. This factory method provides a clear,
    /// intention‑revealing alternative to directly invoking the constructor.
    /// </summary>
    /// <param name="urn">The unique numeric identifier assigned to the establishment.</param>
    /// <param name="name">The display name of the establishment.</param>
    /// <param name="address">The postal address of the establishment.</param>
    /// <param name="type">The establishment type classification.</param>
    /// <param name="group">The group‑level details associated with the establishment.</param>
    /// <param name="localAuthority">The local authority responsible for the establishment.</param>
    /// <returns>
    /// A fully populated <see cref="EstablishmentSearchResult"/> instance.
    /// </returns>
    public static EstablishmentSearchResult Create(
        UniqueReferenceNumber urn,
        Name name,
        Address address,
        EstablishmentType type,
        GroupDetail group,
        LocalAuthority localAuthority)
        => new(urn, name, address, type, group, localAuthority);
}
