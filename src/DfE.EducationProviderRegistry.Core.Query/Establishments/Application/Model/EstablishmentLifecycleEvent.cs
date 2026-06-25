namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public record EstablishmentLifecycleEvent(
    EstablishmentLifecycleEventType EventType,
    DateOnly EventDate,
    EstablishmentLifeCycleReason Reason);

public record EstablishmentLifeCycleReason(string Reason);

public enum EstablishmentLifecycleEventType
{
    Opened,
    Closed
}
