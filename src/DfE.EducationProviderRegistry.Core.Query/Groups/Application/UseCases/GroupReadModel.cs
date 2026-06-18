using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases;

public sealed record GroupReadModel
{
    public required string GroupId { get; init; }
    public required int GroupUID { get; init; }
    public required string CompaniesHouseId { get; init; }
    public required IReadOnlyCollection<Academy> Academies { get; init; }
    public required IReadOnlyCollection<MemberReadModel> Members { get; init; }
    public required IReadOnlyCollection<TrusteeReadModel> Trustees { get; init; }
}
