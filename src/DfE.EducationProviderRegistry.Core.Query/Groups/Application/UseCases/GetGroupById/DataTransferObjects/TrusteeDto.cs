using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;

public sealed record TrusteeDto
{
    public required string Id { get; init; }
    public required string FullName { get; init; }
    public required DateTime StartDate { get; init; }
    public TrusteeTitleType? Title { get; init; }
}
