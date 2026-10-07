namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record EstablishmentStatusModel
{
    public EstablishmentStatusModel(EstablishmentStatus status, DateOnly? effectiveDate)
    {
        Status = status;
        EffectiveDate = effectiveDate;
    }

    public EstablishmentStatus Status { get; }
    public DateOnly? EffectiveDate { get; }

}

public enum EstablishmentStatus
{
    Open = 1,
    Closed = 2,
    ProposedToOpen = 3,
    OpenButProposedToClose = 4
}
