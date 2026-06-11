using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.DataTransferObjects;

public sealed record GroupDto
{
    public required string GroupId { get; init; }
    public required int GroupUID { get; init; }
    public required string CompaniesHouseId { get; init; }
    public required IReadOnlyCollection<Academy> Academies { get; init; }
    public required IReadOnlyCollection<MemberDto> Members { get; init; }
    public required IReadOnlyCollection<TrusteeDto> Trustees { get; init; }
}
