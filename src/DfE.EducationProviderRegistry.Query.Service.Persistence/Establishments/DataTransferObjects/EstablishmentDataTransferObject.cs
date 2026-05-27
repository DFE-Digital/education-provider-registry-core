namespace DfE.EducationProviderRegistry.Query.Service.Persistence.Establishments.DataTransferObjects;

/// <summary>
/// Represents the raw establishment data as retrieved from the data source.
/// This Data Transfer Object (DTO) is used exclusively for persistence and
/// transport concerns and contains no domain logic.
/// </summary>
public sealed class EstablishmentDataTransferObject
{
    /// <summary>
    /// Gets or sets the unique numeric identifier (URN) assigned to the establishment.
    /// </summary>
    public required string URN { get; set; }
}
