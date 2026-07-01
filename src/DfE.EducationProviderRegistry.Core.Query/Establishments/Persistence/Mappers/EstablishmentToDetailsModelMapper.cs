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

        return new EstablishmentDetailsModel
        {
            Urn = EstablishmentUrnModel.Create(dto.Urn.ToString()),
            Name = new EstablishmentNameModel(dto.Name),
            Number = new EstablishmentNumberModel(dto.EstablishmentNumber),
            Status = new EstablishmentStatusModel(dto.EstablishmentStatus.Name),
            Type = new EstablishmentTypeModel(dto.EstablishmentType.Name),
            Phase = new PhaseOfEducationModel(dto.EstablishmentProvision.EducationPhase.Name),
            LifecycleEventOpened = dto.EstablishmentLifecycleEvent
                    .Where(l => l.EventType == "Opened")
                    .Select(l => new EstablishmentLifecycleEventModel(
                        EstablishmentLifecycleEventType.Opened,
                        l.EventDate,
                        new EstablishmentLifeCycleReason(l.OpenedReason.Name)))
                    .FirstOrDefault(), // TODO: Handle getting specific lifecycle event types in a more robust way
            LifecycleEventClosed = dto.EstablishmentLifecycleEvent
                    .Where(l => l.EventType == "Closed")
                    .Select(l => new EstablishmentLifecycleEventModel(
                        EstablishmentLifecycleEventType.Closed,
                        l.EventDate,
                        new EstablishmentLifeCycleReason(l.ClosedReason.Name)))
                    .FirstOrDefault(), // TODO: Handle getting specific lifecycle event types in a more robust way
            Governors = new List<GovernorModel>(),
        };
    }
}
