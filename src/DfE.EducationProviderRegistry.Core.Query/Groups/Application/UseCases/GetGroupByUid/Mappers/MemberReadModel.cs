namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;

public sealed record MemberReadModel
{
    public required string Identifier { get; init; }
    public required string FullName { get; init; }
    public required DateTime StartDate { get; init; }
}
