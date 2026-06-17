using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;

/// <summary>
/// Maps a single <see cref="EstablishmentDto"/> into a fully
/// constructed domain <see cref="Establishment"/> instance.
/// </summary>
/// <remarks>
/// This mapper applies all domain validation rules via the injected validators.
/// It is responsible only for transformation and does not perform any persistence
/// or repository operations.
/// </remarks>
public sealed class EstablishmentDtoToModelMapper :
    IMapper<EstablishmentDto, Establishment>
{

    /// <summary>
    /// Maps the supplied <see cref="EstablishmentDto"/> into a corresponding
    /// <see cref="Establishment"/> domain model.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object containing establishment information.
    /// </param>
    /// <returns>
    /// A fully constructed <see cref="Establishment"/> domain object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dto"/> is <c>null</c>.
    /// </exception>
    public Establishment Map(EstablishmentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new Establishment
        {
            Urn = EstablishmentUrn.Create(dto.URN),
            Ukprn = new Ukprn(dto.UKPRN),
            Uprn = new EstablishmentUprn(dto.UPPN),
            Name = new EstablishmentName(dto.Name),
            Number = new EstablishmentNumber(dto.Number),
            Status = new EstablishmentStatus(dto.Status),
            Type = new EstablishmentType(dto.Type),
            Phase = new PhaseOfEducation(dto.PhaseOfEducation),
            OpenDate = new EstablishmentOpenDate(dto.OpenDate),
            ReasonEstablishmentOpened = new EstablishmentOpenReason(dto.ReasonEstablishmentOpened),
            CloseDate = dto.CloseDate is not null ? new EstablishmentCloseDate(dto.CloseDate) : null,
            ReasonEstablishmentClosed = dto.ReasonEstablishmentClosed is not null ? new EstablishmentCloseReason(dto.ReasonEstablishmentClosed) : null,
            Address = new Address(
                    dto.Address.Street,
                    dto.Address.Town,
                    dto.Address.County,
                    dto.Address.Postcode),
            Governors = dto.Governors?.Select(g => new Governor(
                new GovernancePersonInfo(
                new GovernanceIdentifier(g.Identifier),
                new Name(g.FullName),
                g.StartDate))).ToList() ?? new List<Governor>()
        };
    }
}
