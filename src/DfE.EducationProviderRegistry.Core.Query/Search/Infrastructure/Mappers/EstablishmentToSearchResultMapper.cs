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

        UniqueReferenceNumber urn = new(input.Urn);
        Name name = new(input.Name);

        Site site = input.Site.FirstOrDefault()
            ?? throw new InvalidOperationException(
                "Establishment.Site must contain at least one site.");

        Address address =
            new(
                Street: site.AddressLine1,
                Town: site.Town,
                County: site.County,
                Postcode: site.Postcode
            );

        EstablishmentType type = new(input.EstablishmentType.Name);

        EstablishmentGroupMembership membership = input.EstablishmentGroupMembership.FirstOrDefault()
            ?? throw new InvalidOperationException(
                "Establishment.EstablishmentGroupMembership must contain at least one group membership.");

        GroupRecord groupRecord = membership.Group
            ?? throw new InvalidOperationException(
                "GroupRecord cannot be null.");

        GroupDetail group =
            new(
                partOfName: groupRecord.Name,
                partOfCode: groupRecord.Code
            );

        EstablishmentAuthority authority =
            input.EstablishmentAuthority.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    "Establishment.EstablishmentAuthority must contain at least one authority.");

        LocalAuthority localAuthority = new(
            localAuthorityCode: authority.AuthorityCode,
            localAuthorityName: authority.AuthorityName
        );

        return new EstablishmentSearchResult(
            urn,
            name,
            address,
            type,
            group,
            localAuthority
        );
    }
}
