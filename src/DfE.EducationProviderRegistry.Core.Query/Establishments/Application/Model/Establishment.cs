using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record Establishment
{
    public EstablishmentUrn Urn { get; init; }
    public Ukprn Ukprn { get; init; }
    public EstablishmentUprn Uprn { get; init; }

    public EstablishmentName Name { get; init; }
    public EstablishmentNumber Number { get; init; }
    public Address? Address { get; init; }

    public EstablishmentStatus Status { get; init; }
    public EstablishmentType Type { get; init; }
    public PhaseOfEducation Phase { get; init; }

    public EstablishmentOpenDate OpenDate { get; init; }
    public EstablishmentOpenReason ReasonEstablishmentOpened { get; init; }
    public EstablishmentCloseDate? CloseDate { get; init; }
    public EstablishmentCloseReason? ReasonEstablishmentClosed { get; init; }

    public IEnumerable<Governor> Governors { get; init; }
}
