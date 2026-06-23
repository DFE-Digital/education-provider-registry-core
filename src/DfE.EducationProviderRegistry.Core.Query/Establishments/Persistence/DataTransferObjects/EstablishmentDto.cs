using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;

/// <summary>
/// Represents the raw establishment data as retrieved from the data source.
/// This Data Transfer Object (DTO) is used exclusively for persistence and
/// transport concerns and contains no domain logic.
/// </summary>
public sealed class EstablishmentDto
{
    public required string URN { get; set; }
    public string UKPRN { get; set; }
    public string UPPN { get; set; }

    public string Name { get; set; }
    public string Number { get; set; }
    public string Status { get; set; }

    public DateTime OpenDate { get; set; }
    public string ReasonEstablishmentOpened { get; set; }
    public DateTime? CloseDate { get; set; }
    public string? ReasonEstablishmentClosed { get; set; }

    public AddressDto? Address { get; set; }

    public string Type { get; set; }
    public string PhaseOfEducation { get; set; }

    public IReadOnlyCollection<GovernorDto> Governors { get; set; }
}


[Table("establishment", Schema = "core")]
public class EfEstablishmentDto
{
    [Key]
    [Column("establishment_id")]
    public long EstablishmentId { get; set; }

    [Column("urn")]
    public string? Urn { get; set; }

    [Column("uid")]
    public string? Uid { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("establishment_number")]
    public string? EstablishmentNumber { get; set; }

    [Column("establishment_type_id")]
    public long EstablishmentTypeId { get; set; }

    [Column("establishment_status_id")]
    public long EstablishmentStatusId { get; set; }

    [Column("headteacher_role_assignment_id")]
    public long? HeadteacherRoleAssignmentId { get; set; }

    public EfEstablishmentTypeDto EstablishmentType { get; set; }
    public EfEstablishmentStatusDto EstablishmentStatus { get; set; }
    public EfEstablishmentProvisionDto? Provision { get; set; }

    public ICollection<EfSiteDto> Sites { get; set; }
        = new List<EfSiteDto>();

    public ICollection<EfContactDto> Contacts { get; set; }
        = new List<EfContactDto>();

    public ICollection<EfEstablishmentLifecycleEventDto> LifecycleEvents { get; set; }
        = new List<EfEstablishmentLifecycleEventDto>();

    public ICollection<EfEstablishmentStatusHistoryDto> StatusHistory { get; set; }
        = new List<EfEstablishmentStatusHistoryDto>();

    public ICollection<EfEstablishmentInspectionDto> Inspections { get; set; }
        = new List<EfEstablishmentInspectionDto>();

    public EfEstablishmentAdmissionsDto? Admissions { get; set; }
    public EfEstablishmentAuthorityDto? Authority { get; set; }
    public EfEstablishmentReligionDto? Religion { get; set; }
}

[Table("site", Schema = "core")]
public class EfSiteDto
{
    [Key]
    [Column("site_id")]
    public long SiteId { get; set; }

    [Column("establishment_id")]
    public long EstablishmentId { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("address_line_1")]
    public string? AddressLine1 { get; set; }

    [Column("address_line_2")]
    public string? AddressLine2 { get; set; }

    [Column("town")]
    public string? Town { get; set; }

    [Column("county")]
    public string? County { get; set; }

    [Column("postcode")]
    public string? Postcode { get; set; }

    // Navigation
    public EfEstablishmentDto Establishment { get; set; }
}

[Table("contact", Schema = "core")]
public class EfContactDto
{
    [Key]
    [Column("contact_id")]
    public long ContactId { get; set; }

    [Column("establishment_id")]
    public long? EstablishmentId { get; set; }

    [Column("group_id")]
    public long? GroupId { get; set; }

    [Column("website")]
    public string? Website { get; set; }

    [Column("telephone_number")]
    public string? TelephoneNumber { get; set; }

    // Navigation
    public EfEstablishmentDto? Establishment { get; set; }
}

[Table("establishment_provision", Schema = "core")]
public class EfEstablishmentProvisionDto
{
    [Key]
    [Column("establishment_provision_id")]
    public long EstablishmentProvisionId { get; set; }

    [Column("establishment_id")]
    public long EstablishmentId { get; set; }

    [Column("education_phase_id")]
    public long? EducationPhaseId { get; set; }

    [Column("nursery_provision_id")]
    public long? NurseryProvisionId { get; set; }

    [Column("official_sixth_form_id")]
    public long? OfficialSixthFormId { get; set; }

    [Column("further_education_type_id")]
    public long? FurtherEducationTypeId { get; set; }

    [Column("fsm")]
    public int? Fsm { get; set; }

    [Column("percentage_fsm")]
    public decimal? PercentageFsm { get; set; }

    // Navigation
    public EfEstablishmentDto Establishment { get; set; }
    public EfEducationPhaseDto? EducationPhase { get; set; }
}

[Table("establishment_status_history", Schema = "core")]
public class EfEstablishmentStatusHistoryDto
{
    [Key]
    [Column("establishment_status_history_id")]
    public long EstablishmentStatusHistoryId { get; set; }

    [Column("establishment_id")]
    public long EstablishmentId { get; set; }

    [Column("old_status_id")]
    public long? OldStatusId { get; set; }

    [Column("new_status_id")]
    public long NewStatusId { get; set; }

    [Column("changed_at")]
    public DateTimeOffset ChangedAt { get; set; }

    // Navigation
    public EfEstablishmentDto Establishment { get; set; }
    public EfEstablishmentStatusDto? OldStatus { get; set; }
    public EfEstablishmentStatusDto NewStatus { get; set; }
}

[Table("establishment_lifecycle_event", Schema = "core")]
public class EfEstablishmentLifecycleEventDto
{
    [Key]
    [Column("establishment_lifecycle_event_id")]
    public long EstablishmentLifecycleEventId { get; set; }

    [Column("establishment_id")]
    public long EstablishmentId { get; set; }

    [Column("event_type")]
    public string EventType { get; set; } = string.Empty;

    [Column("opened_reason_id")]
    public long? OpenedReasonId { get; set; }

    [Column("closed_reason_id")]
    public long? ClosedReasonId { get; set; }

    [Column("event_date")]
    public DateOnly EventDate { get; set; }

    // Navigation
    public EfEstablishmentDto Establishment { get; set; }
    public EfReasonEstablishmentOpenedDto? OpenedReason { get; set; }
    public EfReasonEstablishmentClosedDto? ClosedReason { get; set; }
}

[Table("establishment_authority", Schema = "core")]
public class EfEstablishmentAuthorityDto
{
    [Key]
    [Column("establishment_authority_id")]
    public long EstablishmentAuthorityId { get; set; }

    [Column("establishment_id")]
    public long EstablishmentId { get; set; }

    [Column("authority_code")]
    public string? AuthorityCode { get; set; }

    [Column("authority_name")]
    public string? AuthorityName { get; set; }

    // Navigation
    public EfEstablishmentDto Establishment { get; set; }
}

[Table("establishment_admissions", Schema = "core")]
public class EfEstablishmentAdmissionsDto
{
    [Key]
    [Column("establishment_admissions_id")]
    public long EstablishmentAdmissionsId { get; set; }

    [Column("establishment_id")]
    public long EstablishmentId { get; set; }

    [Column("admissions_policy")]
    public string? AdmissionsPolicy { get; set; }

    [Column("statutory_low_age")]
    public int? StatutoryLowAge { get; set; }

    [Column("statutory_high_age")]
    public int? StatutoryHighAge { get; set; }

    // Navigation
    public EfEstablishmentDto Establishment { get; set; }
}

[Table("establishment_religion", Schema = "core")]
public class EfEstablishmentReligionDto
{
    [Key]
    [Column("establishment_religion_id")]
    public long EstablishmentReligionId { get; set; }

    [Column("establishment_id")]
    public long? EstablishmentId { get; set; }

    [Column("group_id")]
    public long? GroupId { get; set; }

    [Column("religious_character")]
    public string? ReligiousCharacter { get; set; }

    [Column("religious_ethos")]
    public string? ReligiousEthos { get; set; }

    // Navigation
    public EfEstablishmentDto? Establishment { get; set; }
}

[Table("establishment_inspection", Schema = "core")]
public class EfEstablishmentInspectionDto
{
    [Key]
    [Column("establishment_inspection_id")]
    public long EstablishmentInspectionId { get; set; }

    [Column("establishment_id")]
    public long EstablishmentId { get; set; }

    [Column("inspection_body")]
    public string? InspectionBody { get; set; }

    [Column("inspection_date")]
    public DateOnly? InspectionDate { get; set; }

    [Column("inspection_outcome")]
    public string? InspectionOutcome { get; set; }

    // Navigation
    public EfEstablishmentDto Establishment { get; set; }
}

// References
[Table("establishment_type", Schema = "ref")]
public class EfEstablishmentTypeDto
{
    [Key]
    [Column("establishment_type_id")]
    public long EstablishmentTypeId { get; set; }

    [Column("establishment_family_id")]
    public long EstablishmentFamilyId { get; set; }

    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("is_school")]
    public bool IsSchool { get; set; }

    [Column("is_group")]
    public bool IsGroup { get; set; }

    [Column("is_early_years")]
    public bool IsEarlyYears { get; set; }

    [Column("is_further_education")]
    public bool IsFurtherEducation { get; set; }

    public ICollection<EfEstablishmentDto> Establishments { get; set; }
        = new List<EfEstablishmentDto>();
}

[Table("establishment_status", Schema = "ref")]
public class EfEstablishmentStatusDto
{
    [Key]
    [Column("establishment_status_id")]
    public long EstablishmentStatusId { get; set; }

    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    public ICollection<EfEstablishmentDto> Establishments { get; set; }
        = new List<EfEstablishmentDto>();
}

[Table("education_phase", Schema = "ref")]
public class EfEducationPhaseDto
{
    [Key]
    [Column("education_phase_id")]
    public long EducationPhaseId { get; set; }

    [Column("education_phase_group_id")]
    public long EducationPhaseGroupId { get; set; }

    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public EfEducationPhaseGroupDto EducationPhaseGroup { get; set; }

    public ICollection<EfEstablishmentProvisionDto> EstablishmentProvisions { get; set; }
        = new List<EfEstablishmentProvisionDto>();
}

[Table("education_phase_group", Schema = "ref")]
public class EfEducationPhaseGroupDto
{
    [Key]
    [Column("education_phase_group_id")]
    public long EducationPhaseGroupId { get; set; }

    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public ICollection<EfEducationPhaseDto> EducationPhases { get; set; }
        = new List<EfEducationPhaseDto>();
}

[Table("reason_establishment_opened", Schema = "ref")]
public class EfReasonEstablishmentOpenedDto
{
    [Key]
    [Column("reason_establishment_opened_id")]
    public long ReasonEstablishmentOpenedId { get; set; }

    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public ICollection<EfEstablishmentLifecycleEventDto> LifecycleEvents { get; set; }
        = new List<EfEstablishmentLifecycleEventDto>();
}

[Table("reason_establishment_closed", Schema = "ref")]
public class EfReasonEstablishmentClosedDto
{
    [Key]
    [Column("reason_establishment_closed_id")]
    public long ReasonEstablishmentClosedId { get; set; }

    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public ICollection<EfEstablishmentLifecycleEventDto> LifecycleEvents { get; set; }
        = new List<EfEstablishmentLifecycleEventDto>();
}

