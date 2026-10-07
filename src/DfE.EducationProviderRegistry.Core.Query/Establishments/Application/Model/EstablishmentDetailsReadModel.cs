namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record class EstablishmentDetailsReadModel
{
    public required EstablishmentDetailsModel? Establishment { get; init; }
    public IReadOnlyCollection<GovernorModel> Governors { get; init; } = [];
}
