using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;

public sealed class EstablishmentToDetailsModelMapper :
    IMapper<Establishment, EstablishmentDetailsModel>
{
    public EstablishmentDetailsModel Map(Establishment dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        EstablishmentUrnModel urn = EstablishmentUrnModel.Create(dto.Urn.ToString());
        EstablishmentNameModel name = new(dto.Name);
        EstablishmentNumberModel number = new(dto.EstablishmentNumber);
        EstablishmentStatusModel status = new(dto.EstablishmentStatus.Name);
        EstablishmentTypeModel type = new(dto.EstablishmentType.Name);
        PhaseOfEducationModel phase = new(dto.EstablishmentProvision.EducationPhase.Name);

        EstablishmentLifecycleEventModel? openedEvent = dto.EstablishmentLifecycleEvent
            .Where(e => e.EventType == "Opened")
            .Select(e => new EstablishmentLifecycleEventModel(
                EstablishmentLifecycleEventType.Opened,
                e.EventDate,
                new EstablishmentLifeCycleReason(e.OpenedReason!.Name)))
            .FirstOrDefault();

        EstablishmentLifecycleEventModel? closedEvent = dto.EstablishmentLifecycleEvent
            .Where(e => e.EventType == "Closed")
            .Select(e => new EstablishmentLifecycleEventModel(
                EstablishmentLifecycleEventType.Closed,
                e.EventDate,
                new EstablishmentLifeCycleReason(e.ClosedReason!.Name)))
            .FirstOrDefault();

        return new EstablishmentDetailsModel
        {
            Urn = urn,
            Name = name,
            Number = number,
            Status = status,
            Type = type,
            Phase = phase,
            LifecycleEventOpened = openedEvent,
            LifecycleEventClosed = closedEvent,
            Governors = new List<GovernorModel>()
        };
    }
}
