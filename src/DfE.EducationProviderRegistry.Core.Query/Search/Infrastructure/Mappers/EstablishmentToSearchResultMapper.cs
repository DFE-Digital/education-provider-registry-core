using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using EstablishmentType = DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment.EstablishmentType;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;

/// <summary>
/// Maps an <see cref="Establishment"/> database entity into an
/// <see cref="EstablishmentSearchResult"/> projection used by the search layer.
/// </summary>
internal sealed class EstablishmentToSearchResultMapper
    : IMapper<Establishment, EstablishmentSearchResult>
{
    /// <summary>
    /// Creates a search result projection from the supplied establishment.
    /// Handles optional related entities (site, group, authority) gracefully.
    /// </summary>
    /// <param name="input">The establishment entity to map.</param>
    /// <returns>A populated <see cref="EstablishmentSearchResult"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is null.
    /// </exception>
    public EstablishmentSearchResult Map(Establishment input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Site? site = input.Site.FirstOrDefault();

        SiteAddressModel? address = site is null
            ? null
            : new SiteAddressModel(
                Name: site.Name ?? string.Empty,
                AddressLine1: site.AddressLine1!,
                AddressLine2: site.AddressLine2!,
                Town: site.Town!,
                County: site.County!,
                Postcode: site.Postcode!);

        EstablishmentGroupMembership? membership =
            input.EstablishmentGroupMembership.FirstOrDefault();

        GroupDetail? group = membership is null
            ? null
            : new GroupDetail(
                partOfName: membership.Group?.Name!,
                partOfCode: membership.Group?.Code!);

        EstablishmentAuthority? authority =
            input.EstablishmentAuthority.FirstOrDefault();

        LocalAuthority? localAuthority = authority is null
            ? null
            : new LocalAuthority(
                localAuthorityName: authority.AuthorityName!,
                localAuthorityCode: authority.AuthorityCode!);

        EstablishmentType? type = input.EstablishmentType is null
            ? null
            : new EstablishmentType(input.EstablishmentType.Name);

        return new EstablishmentSearchResult(
            new UniqueReferenceNumber(input.Urn!),
            new Name(input.Name),
            address,
            type,
            group,
            localAuthority);
    }
}
