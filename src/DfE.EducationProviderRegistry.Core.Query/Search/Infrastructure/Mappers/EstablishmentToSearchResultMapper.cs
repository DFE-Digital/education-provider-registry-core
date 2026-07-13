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

        Site? site = input.Site.FirstOrDefault();

        Address? address = site != null ?
            new(
                Street: site.AddressLine1,
                Town: site.Town,
                County: site.County,
                Postcode: site.Postcode
            ) : null;

        EstablishmentType type = new(input.EstablishmentType.Name);

        GroupRecord? groupRecord = input.EstablishmentGroupMembership.FirstOrDefault()?.Group;

        GroupDetail? group = groupRecord != null ?
            new(
                partOfName: groupRecord.Name,
                partOfCode: groupRecord.Code
            ) : null;

        EstablishmentAuthority? authority =
            input.EstablishmentAuthority.FirstOrDefault();

        LocalAuthority? localAuthority = authority != null ? new(
            localAuthorityCode: authority.AuthorityCode,
            localAuthorityName: authority.AuthorityName
        ) : null;

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
