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

        Address address = new(
            Street: site?.AddressLine1,
            Town: site?.Town,
            County: site?.County,
            Postcode: site?.Postcode
        );

        EstablishmentGroupMembership? membership =
            input.EstablishmentGroupMembership.FirstOrDefault();

        GroupDetail group = new(
            partOfName: membership?.Group?.Name,
            partOfCode: membership?.Group?.Code
        );

        EstablishmentAuthority? authority =
            input.EstablishmentAuthority.FirstOrDefault();

        LocalAuthority localAuthority = new(
            localAuthorityCode: authority?.AuthorityCode,
            localAuthorityName: authority?.AuthorityName
        );

        return new EstablishmentSearchResult(
            new UniqueReferenceNumber(input.Urn),
            new Name(input.Name),
            address,
            new EstablishmentType(input.EstablishmentType?.Name),
            group,
            localAuthority
        );
    }
}
