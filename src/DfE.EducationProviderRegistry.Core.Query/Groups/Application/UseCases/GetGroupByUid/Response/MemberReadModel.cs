namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

public sealed record MemberReadModel
{
    public required string Identifier { get; init; }
    public required string FullName { get; init; }
    public required DateOnly StartDate { get; init; }
    public string? Title { get; init; }
    public DateOnly? EndDate { get; init; }
    public required string? AppointedBy { get; init; }
}
