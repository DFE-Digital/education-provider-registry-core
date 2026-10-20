using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid;

public sealed record GroupReadModel
{
    public required string Name { get; init; }
    public required string GroupId { get; init; }
    public required long GroupUID { get; init; }
    public required IReadOnlyCollection<Academy> Academies { get; init; }
    public required IReadOnlyCollection<MemberReadModel> Members { get; init; }
    public required IReadOnlyCollection<TrusteeReadModel> Trustees { get; init; }
    public string? UKPRN { get; init; }
    public string? CompaniesHouseId { get; init; }
    public string? Address { get; init; }
    public string? Status { get; init; }
    public string? Type { get; init; }
}

public sealed record GroupReadModelResponse
{
    public required GroupReadModel? Group { get; init; }
}
