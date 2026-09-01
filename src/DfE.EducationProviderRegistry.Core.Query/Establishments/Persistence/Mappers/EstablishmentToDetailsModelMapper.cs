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

        EstablishmentUrnModel urn = EstablishmentUrnModel.Create(dto.Urn!);
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
        EstablishmentGroupModel? group = new(groupMembership?.Group?.Name ?? string.Empty, groupMembership?.Group?.Code ?? string.Empty);
        string? groupType = groupMembership?.Group.GroupType.Name;

        DateOnly? groupOpenDate = groupMembership?.StartDate;

        Site? site = dto.Site.FirstOrDefault();
        SiteAddressModel? address = site is null
            ? null
            : new SiteAddressModel(
                Name: site.Name ?? string.Empty,
                AddressLine1: site.AddressLine1 ?? string.Empty,
                AddressLine2: site.AddressLine2 ?? string.Empty,
                Town: site.Town ?? string.Empty,
                County: site.County ?? string.Empty,
                Postcode: site.Postcode ?? string.Empty
            );

        LocalAuthority? localAuthority = dto.EstablishmentAuthority.FirstOrDefault() is EstablishmentAuthority authority
            ? new LocalAuthority(authority.AuthorityName!, authority.AuthorityCode!)
            : null;

        string? religiousCharacter = dto.EstablishmentReligion.FirstOrDefault()?.ReligiousCharacter;
        EstablishmentInspection? ofsted = dto.EstablishmentInspection.FirstOrDefault();

        string? ageRange = dto.EstablishmentAdmissions?.StatutoryLowAge is not null && dto.EstablishmentAdmissions.StatutoryHighAge is not null
            ? $"{dto.EstablishmentAdmissions.StatutoryLowAge} to {dto.EstablishmentAdmissions.StatutoryHighAge}"
            : null;

        List<GovernorModel> governors = [.. dto.RoleAssignment
            .Where(ra => ra.Role?.Person != null)
            .Select(ra => new GovernorModel(
                Identifier: new GovernanceIdentifier(ra.Role?.Person?.PersonId.ToString()),
                Name: new Name(ra.Role?.Person?.DisplayName ?? string.Empty)
            ))];

        //TODO: update role type code mapping to not use magic string -- probably worth waiting for new schema to be released before doing this as it will possibly change the way we map role types
        string? headTeacherDisplayName = dto.RoleAssignment?.FirstOrDefault(x => x.Role?.RoleType.Code == "HT")?.Role?.Person?.DisplayName ?? null;

        string? senProvision = dto.EstablishmentSen?.SenProvision ?? null;

        EstablishmentContactDetails? contactDetails = dto.Contact.FirstOrDefault() is not null
            ? new(dto.Contact.FirstOrDefault()?.Website ?? string.Empty, dto.Contact.FirstOrDefault()?.TelephoneNumber ?? string.Empty)
            : null;

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
            Group = group,
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
