using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using EstablishmentType = DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment.EstablishmentType;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Mappers;

internal sealed class EstablishmentToSearchResultMapper : IMapper<Establishment, EstablishmentSearchResult>
{
    public EstablishmentSearchResult Map(Establishment input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Site? site = input.Site.FirstOrDefault();

        Address? address = site is null
            ? null
            : new Address(
                Street: site.AddressLine1 ?? string.Empty,
                Town: site.Town ?? string.Empty,
                County: site.County ?? string.Empty,
                Postcode: site.Postcode ?? string.Empty);

        EstablishmentGroupMembership? membership =
            input.EstablishmentGroupMembership.FirstOrDefault();

        GroupDetail? group = membership is null
            ? null
            : new GroupDetail(
                partOfName: membership.Group?.Name ?? string.Empty,
                partOfCode: membership.Group?.Code ?? string.Empty);

        EstablishmentAuthority? authority =
            input.EstablishmentAuthority.FirstOrDefault();

        LocalAuthority? localAuthority = authority is null
            ? null
            : new LocalAuthority(
                localAuthorityName: authority.AuthorityName ?? string.Empty,
                localAuthorityCode: authority.AuthorityCode ?? string.Empty);

        EstablishmentType? type = input.EstablishmentType is null
            ? null
            : new EstablishmentType(input.EstablishmentType.Name);

        return new EstablishmentSearchResult(
            new UniqueReferenceNumber(input.Urn ?? string.Empty),
            new Name(input.Name),
            address,
            type,
            group,
            localAuthority);
    }
}
