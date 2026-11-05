namespace DfE.EducationProviderRegistry.Core.Query.Download.Shared.Infrastructure.QueryEngine.DataTransferObjects;

internal class EstablishmentDownloadDto
{
    public string? EstablishmentUniqueReferenceNumberUrn { get; set; }
    public string? EstablishmentNumber { get; set; }
    public string? EstablishmentName { get; set; }
    public string? EstablishmentTypeName { get; set; }
    public string? EducationPhaseName { get; set; }
}
