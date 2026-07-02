namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record EstablishmentDetailsModel
{
    public EstablishmentUrnModel Urn { get; init; }

    public EstablishmentNameModel Name { get; init; }
    public EstablishmentNumberModel Number { get; init; }

    public EstablishmentStatusModel Status { get; init; }
    public EstablishmentTypeModel Type { get; init; }
    public PhaseOfEducationModel Phase { get; init; }

    public EstablishmentLifecycleEventModel? LifecycleEventOpened { get; init; }
    public EstablishmentLifecycleEventModel? LifecycleEventClosed { get; init; }

    public IEnumerable<GovernorModel> Governors { get; init; }

    public EstablishmentDetailsModel()
    {

    }
}
