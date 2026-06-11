namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;


public sealed record MemberDto
{
    public required string Identifier { get; init; }
    public required string FullName { get; init; }
    public required DateTime StartDate { get; init; }
}
