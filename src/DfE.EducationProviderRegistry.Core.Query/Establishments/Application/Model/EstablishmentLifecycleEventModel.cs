namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public record EstablishmentLifecycleEventModel(
    EstablishmentLifecycleEventType EventType,
    DateOnly EventDate,
    EstablishmentLifeCycleReason Reason);

public record EstablishmentLifeCycleReason(string? Reason);

public enum EstablishmentLifecycleEventType
{
    Opened,
    Closed
}
