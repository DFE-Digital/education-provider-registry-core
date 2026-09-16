using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;

/// <summary>
/// Represents a single search provider result returned as part of a search operation.
/// This model contains presentation‑ready fields optimised for lightweight
/// transport between the search layer and consuming clients.
/// </summary>
public sealed record SearchAggregateResult
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
    public SearchAddress? Address { get; }

    /// <summary>
    /// Gets the search result provider type classification (e.g., Academy, Free School).
    /// </summary>
    public SearchType? Type { get; }

    /// <summary>
    /// Gets the group‑level details associated with the search result provider,
    /// such as trust or federation information.
    /// </summary>
    public GroupDetail? Group { get; }

    /// <summary>
    /// Gets the local authority responsible for the search result provider.
    /// </summary>
    public SearchLocalAuthority? LocalAuthority { get; }

    /// <summary>
    /// Gets the provider category (i.e. 'Group' OR 'Etsablishment') for the search result provider.
    /// </summary>
    public SearchCategory ProviderCategory { get; }

    /// <summary>
    /// Gets the total number of academies associated with the search result provider. O
    /// Only applicable for group providers. For search providers, this value will be zero.
    /// </summary>
    public int AcademyCount { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchAggregateResult"/> record
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
    public SearchAggregateResult(
        ProviderIdentifier uniqueIdentifier,
        Name name,
        SearchAddress? address,
        SearchType? type,
        GroupDetail? group,
        SearchLocalAuthority? localAuthority,
        SearchCategory providerCategory,
        int academyCount)
    {
        UniqueIdentifier = uniqueIdentifier;
        Name = name;
        Address = address;
        Type = type;
        Group = group;
        LocalAuthority = localAuthority;
        ProviderCategory = providerCategory;
        AcademyCount = academyCount;
    }

    /// <summary>
    /// Creates a new <see cref="SearchAggregateResult"/> instance using the
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
    /// A fully populated <see cref="SearchAggregateResult"/> instance.
    /// </returns>
    public static SearchAggregateResult Create(
        ProviderIdentifier uniqueIdentifier,
        Name name,
        SearchAddress? address,
        SearchType? type,
        GroupDetail? group,
        SearchLocalAuthority? localAuthority,
        SearchCategory providerCategory,
        int academyCount)
            => new(uniqueIdentifier, name, address, type, group, localAuthority, providerCategory, academyCount);
}
