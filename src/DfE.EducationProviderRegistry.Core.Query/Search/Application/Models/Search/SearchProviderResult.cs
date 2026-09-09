using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Represents a single search provider result returned as part of a search operation.
/// This model contains presentation‑ready fields optimised for lightweight
/// transport between the search layer and consuming clients.
/// </summary>
public sealed record SearchProviderResult
{
    /// <summary>
    /// Gets the unique provier identifier assigned to the search provider result.
    /// </summary>
    public ProviderIdentifier UniqueIdentifier { get; }

    /// <summary>
    /// Gets the display name of the search provider result.
    /// </summary>
    public Name Name { get; }

    /// <summary>
    /// Gets the postal address associated with the search provider result.
    /// </summary>
    public SearchProviderAddress? Address { get; }

    /// <summary>
    /// Gets the search result provider type classification (e.g., Academy, Free School).
    /// </summary>
    public SearchProviderType? Type { get; }

    /// <summary>
    /// Gets the group‑level details associated with the search result provider,
    /// such as trust or federation information.
    /// </summary>
    public GroupDetail? Group { get; }

    /// <summary>
    /// Gets the local authority responsible for the search result provider.
    /// </summary>
    public SearchProviderLocalAuthority? LocalAuthority { get; }

    /// <summary>
    /// Gets the provider category (i.e. 'Group' OR 'Etsablishment') for the search result provider.
    /// </summary>
    public SearchProviderCategory ProviderCategory { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchProviderResult"/> record
    /// using the specified search result provider attributes.
    /// </summary>
    /// <param name="uniqueIdentifier">The unique numeric identifier assigned to the search result provider.</param>
    /// <param name="name">The display name of the search result provider.</param>
    /// <param name="address">The postal address of the search result provider.</param>
    /// <param name="type">The search result provider type classification.</param>
    /// <param name="group">The group‑level details associated with the search result provider.</param>
    /// <param name="localAuthority">The local authority responsible for the search result provider.</param>
    /// <param name="providerCategory">The provider category for the search result provider.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any required parameter is <c>null</c>.
    /// </exception>
    public SearchProviderResult(
        ProviderIdentifier uniqueIdentifier,
        Name name,
        SearchProviderAddress? address,
        SearchProviderType? type,
        GroupDetail? group,
        SearchProviderLocalAuthority? localAuthority,
        SearchProviderCategory providerCategory)
    {
        UniqueIdentifier = uniqueIdentifier;
        Name = name;
        Address = address;
        Type = type;
        Group = group;
        LocalAuthority = localAuthority;
        ProviderCategory = providerCategory;
    }

    /// <summary>
    /// Creates a new <see cref="SearchProviderResult"/> instance using the
    /// supplied search result provider attributes. This factory method provides a clear,
    /// intention‑revealing alternative to directly invoking the constructor.
    /// </summary>
    /// <param name="uniqueIdentifier">The unique numeric identifier assigned to the search result provider.</param>
    /// <param name="name">The display name of the search result provider.</param>
    /// <param name="address">The postal address of the search result provider.</param>
    /// <param name="type">The search result provider type classification.</param>
    /// <param name="group">The group‑level details associated with the search result provider.</param>
    /// <param name="localAuthority">The local authority responsible for the search result provider.</param>
    /// <param name="providerCategory">The provider category for the search result provider.</param>
    /// <returns>
    /// A fully populated <see cref="SearchProviderResult"/> instance.
    /// </returns>
    public static SearchProviderResult Create(
        ProviderIdentifier uniqueIdentifier,
        Name name,
        SearchProviderAddress? address,
        SearchProviderType? type,
        GroupDetail? group,
        SearchProviderLocalAuthority? localAuthority,
        SearchProviderCategory providerCategory)
            => new(uniqueIdentifier, name, address, type, group, localAuthority, providerCategory);
}
