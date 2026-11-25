namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

public sealed record EstablishmentReadModel
{
    public required string Name { get; init; }
    public required string Urn { get; init; }
}
