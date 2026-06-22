using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById.Mappers;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupById;

public sealed record GroupReadModel
{
    public required string GroupId { get; init; }
    public required int GroupUID { get; init; }
    public required string UKPRN { get; init; }
    public required string? CompaniesHouseId { get; init; }
    // public required GroupStatusStateReadModel Status { get; init; }
    // public required DateTime StatusDate { get; init; }
    // public required string Address { get; init; }
    // public required string GroupType { get; init; }
    public required IReadOnlyCollection<Academy> Academies { get; init; }
    public required IReadOnlyCollection<MemberReadModel> Members { get; init; }
    public required IReadOnlyCollection<TrusteeReadModel> Trustees { get; init; }
}
