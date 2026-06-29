using System.Buffers;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;

/// <summary>
/// Maps a collection of <see cref="EstablishmentDto"/> instances
/// into a read-only collection of domain <see cref="EstablishmentDetailsModel"/> objects.
/// </summary>
/// <remarks>
/// Delegates single‑item mapping to <see cref="IMapper{TMapFrom, TMapTo}"/> and uses
/// <see cref="ArrayPool{T}"/> to minimise allocations.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="EstablishmentsToDetailsModelMapper"/> class.
/// </remarks>
/// <param name="establishmentMapper">
/// The mapper used to convert a single <see cref="EstablishmentDto"/>
/// into a domain <see cref="EstablishmentDetailsModel"/>.
/// </param>
internal sealed class EstablishmentsToDetailsModelMapper :
    IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentDetailsModel>>
{
    /// <summary>
    /// The mapper responsible for converting individual DTOs into domain models.
    /// </summary>
    private readonly IMapper<Establishment, EstablishmentDetailsModel> _establishmentMapper;

    public EstablishmentsToDetailsModelMapper(IMapper<Establishment, EstablishmentDetailsModel> establishmentMapper)
    {
        ArgumentNullException.ThrowIfNull(establishmentMapper);
        _establishmentMapper = establishmentMapper;
    }

    /// <summary>
    /// Maps the supplied DTO collection into a corresponding collection of domain models.
    /// </summary>
    /// <param name="input">
    /// The sequence of <see cref="EstablishmentDto"/> instances to map.
    /// </param>
    /// <returns>
    /// A read-only collection of fully constructed <see cref="EstablishmentDetailsModel"/> domain objects.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is <c>null</c>.
    /// </exception>
    public IReadOnlyCollection<EstablishmentDetailsModel> Map(IEnumerable<Establishment> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input
            .Select(_establishmentMapper.Map)
            .ToArray();
    }
}
