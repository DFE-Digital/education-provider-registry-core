using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

public sealed record GroupReadModel
{
    public required string Name { get; init; }
    public required long GroupUID { get; init; }
    public required string? GroupId { get; init; }
    public required IReadOnlyCollection<Academy> Academies { get; init; }
    public required IReadOnlyCollection<MemberReadModel> Members { get; init; }
    public required IReadOnlyCollection<TrusteeReadModel> Trustees { get; init; }
    public required IReadOnlyCollection<EstablishmentReadModel> Establishments { get; init; }
    public string? UKPRN { get; init; }
    public string? CompaniesHouseId { get; init; }
    public string? Address { get; init; }
    public string? Status { get; init; }
    public string? Type { get; init; }
    public string? StatusLabel { get; init; }
    public DateOnly? OpenedDate { get; init; }
    public DateOnly? ClosedDate { get; init; }
}

public sealed record GroupReadModelResponse
{
    public required GroupReadModel? Group { get; init; }
}
