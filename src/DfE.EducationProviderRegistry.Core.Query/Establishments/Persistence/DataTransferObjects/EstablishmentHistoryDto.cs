namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;

public sealed class EstablishmentHistoryDto
{
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required string URN { get; init; }
}
