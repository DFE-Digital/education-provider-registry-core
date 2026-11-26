using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

public sealed record TrusteeReadModel
{
    public required string Id { get; init; }
    public required string FullName { get; init; }
    public required DateOnly StartDate { get; init; }
    public required string Role { get; init; }
    public string? Title { get; init; }
    public DateOnly? EndDate { get; init; }
    public string? AppointedBy { get; init; }
}
