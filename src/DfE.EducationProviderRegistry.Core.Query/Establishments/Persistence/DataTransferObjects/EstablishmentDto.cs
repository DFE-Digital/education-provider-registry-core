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
    public string DfENumber { get; set; }
    public string Status { get; set; }

    public DateTime? OpenDate { get; set; }
    public string ReasonEstablishmentOpened { get; set; }
    public DateTime? CloseDate { get; set; }
    public string ReasonEstablishmentClosed { get; set; }

    public AddressDto Address { get; set; }

    public string LocalAuthority { get; set; }
    public string? PartOfTrust { get; set; }

    public string Type { get; set; }
    public string PhaseOfEducation { get; set; }
    public string Gender { get; set; }
    public string? ReligiousCharacter { get; set; }

    public IReadOnlyCollection<GovernorDto> Governors { get; set; }
    public IReadOnlyCollection<EstablishmentHistoryDto> History { get; set; }
}
