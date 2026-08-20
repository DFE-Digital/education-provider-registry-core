using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;
                    
public sealed class EstablishmentToDetailsModelMapper :
    IMapper<Establishment, EstablishmentDetailsModel>
{
    public EstablishmentDetailsModel Map(Establishment dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        EstablishmentUrnModel urn = EstablishmentUrnModel.Create(dto.Urn.ToString());
        EstablishmentNameModel name = new(dto.Name);
        EstablishmentNumberModel number = new(dto.EstablishmentNumber);
        EstablishmentStatusModel status = new(dto.EstablishmentStatus.Name);
        EstablishmentTypeModel type = new(dto.EstablishmentType.Name);
        PhaseOfEducationModel phase = new(dto.EstablishmentProvision?.EducationPhase?.Name);

        EstablishmentLifecycleEventModel? openedEvent = dto.EstablishmentLifecycleEvent
            .Where(e => e.EventType == "Opened")
            .Select(e => new EstablishmentLifecycleEventModel(
                EstablishmentLifecycleEventType.Opened,
                e.EventDate,
                new EstablishmentLifeCycleReason(e.OpenedReason?.Name)))
            .FirstOrDefault();

        EstablishmentLifecycleEventModel? closedEvent = dto.EstablishmentLifecycleEvent
            .Where(e => e.EventType == "Closed")
            .Select(e => new EstablishmentLifecycleEventModel(
                EstablishmentLifecycleEventType.Closed,
                e.EventDate,
                new EstablishmentLifeCycleReason(e.ClosedReason?.Name)))
            .FirstOrDefault();

        EstablishmentGroupMembership? groupMembership = dto.EstablishmentGroupMembership.FirstOrDefault();
        string? groupName = groupMembership?.Group.Name;
        string? groupType = groupMembership?.Group.GroupType.Name;
        DateOnly? groupOpenDate = groupMembership?.StartDate;

        Site? site = dto.Site.FirstOrDefault();
        SiteAddressModel? address = site is null
            ? null
            : new SiteAddressModel(
                Name: site.Name,
                AddressLine1: site.AddressLine1,
                AddressLine2: site.AddressLine2,
                Town: site.Town,
                County: site.County,
                Postcode: site.Postcode
            );

        LocalAuthority? localAuthority = dto.EstablishmentAuthority is null ? null : new LocalAuthority(dto.EstablishmentAuthority.FirstOrDefault().AuthorityName, dto.EstablishmentAuthority.FirstOrDefault().AuthorityCode);
        string? religiousCharacter = dto.EstablishmentReligion.FirstOrDefault()?.ReligiousCharacter;
        EstablishmentInspection? ofsted = dto.EstablishmentInspection.FirstOrDefault();

        string? ageRange = dto.EstablishmentAdmissions?.StatutoryLowAge is not null && dto.EstablishmentAdmissions.StatutoryHighAge is not null
            ? $"{dto.EstablishmentAdmissions.StatutoryLowAge} to {dto.EstablishmentAdmissions.StatutoryHighAge}"
            : null;

        List<GovernorModel> governors = [.. dto.RoleAssignment
            .Where(ra => ra.Role?.Person != null)
            .Select(ra => new GovernorModel(
                Identifier: new GovernanceIdentifier(ra.Role.Person.PersonId.ToString()),
                Name: new Name(ra.Role.Person.DisplayName)
            ))];

        //todo update role type code mapping to not use magic string
        string headTeacherDisplayName = dto.RoleAssignment?.FirstOrDefault(x => x.Role?.RoleType.Code == "HT")?.Role?.Person?.DisplayName ?? string.Empty;
        string senProvision = dto.EstablishmentSen?.SenProvision ?? string.Empty;
        EstablishmentContactDetails? contactDetails = new EstablishmentContactDetails(dto.Contact.FirstOrDefault()?.Website ?? string.Empty, dto.Contact.FirstOrDefault()?.TelephoneNumber ?? string.Empty);

        return new EstablishmentDetailsModel
        {
            Urn = urn,
            Name = name,
            Number = number,
            Status = status,
            Type = type,
            Phase = phase,
            LifecycleEventOpened = openedEvent,
            LifecycleEventClosed = closedEvent,
            Uid = dto.Uid,
            GroupName = groupName,
            GroupType = groupType,
            GroupOpenDate = groupOpenDate,
            Address = address,
            LocalAuthority = localAuthority,
            AgeRange = ageRange,
            ReligiousCharacter = religiousCharacter,
            Ofsted = ofsted,
            Governors = governors,
            Headteacher = headTeacherDisplayName,
            SenProvision = senProvision,
            ContactDetails = contactDetails
        };
    }
}
