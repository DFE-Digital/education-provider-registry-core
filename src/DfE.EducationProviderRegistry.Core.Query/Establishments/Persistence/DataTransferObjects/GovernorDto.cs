namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;

public sealed class GovernorDto
{
    public required string GovernorId { get; init; }
    public required string FullName { get; init; }
    public required DateTime StartDate { get; init; }
}
