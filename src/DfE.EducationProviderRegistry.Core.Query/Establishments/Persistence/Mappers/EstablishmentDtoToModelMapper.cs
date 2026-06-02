using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;

/// <summary>
/// Maps a single <see cref="EstablishmentDataTransferObject"/> into a fully
/// constructed domain <see cref="Establishment"/> instance.
/// </summary>
/// <remarks>
/// This mapper applies all domain validation rules via the injected validators.
/// It is responsible only for transformation and does not perform any persistence
/// or repository operations.
/// </remarks>
public sealed class EstablishmentDtoToModelMapper :
    IMapper<EstablishmentDataTransferObject, Establishment>
{

    /// <summary>
    /// Maps the supplied <see cref="EstablishmentDataTransferObject"/> into a corresponding
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
    public Establishment Map(EstablishmentDataTransferObject dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        // Construct identifier
        EstablishmentIdentifier identifier = new EstablishmentIdentifier(dto.URN);

        // Construct final aggregate
        return new Establishment(identifier);
    }
}
