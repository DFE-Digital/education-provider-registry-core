using System.Buffers;
using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.Service.Establishments.Persistence.Mappers;

/// <summary>
/// Maps a collection of <see cref="EstablishmentDataTransferObject"/> instances
/// into a read-only collection of domain <see cref="Establishment"/> objects.
/// </summary>
/// <remarks>
/// Delegates single‑item mapping to <see cref="IMapper{TMapFrom, TMapTo}"/> and uses
/// <see cref="ArrayPool{T}"/> to minimise allocations.
/// </remarks>
public sealed class EstablishmentsDtoToModelMapper :
    IMapper<IEnumerable<EstablishmentDataTransferObject>, IReadOnlyCollection<Establishment>>
{
    /// <summary>
    /// The mapper responsible for converting individual DTOs into domain models.
    /// </summary>
    private readonly IMapper<EstablishmentDataTransferObject, Establishment> _establishmentMapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentsDtoToModelMapper"/> class.
    /// </summary>
    /// <param name="establishmentMapper">
    /// The mapper used to convert a single <see cref="EstablishmentDataTransferObject"/>
    /// into a domain <see cref="Establishment"/>.
    /// </param>
    public EstablishmentsDtoToModelMapper(
        IMapper<EstablishmentDataTransferObject, Establishment> establishmentMapper)
    {
        _establishmentMapper = establishmentMapper;
    }

    /// <summary>
    /// Maps the supplied DTO collection into a corresponding collection of domain models.
    /// </summary>
    /// <param name="input">
    /// The sequence of <see cref="EstablishmentDataTransferObject"/> instances to map.
    /// </param>
    /// <returns>
    /// A read-only collection of fully constructed <see cref="Establishment"/> domain objects.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is <c>null</c>.
    /// </exception>
    public IReadOnlyCollection<Establishment> Map(IEnumerable<EstablishmentDataTransferObject> input)
    {
        ArgumentNullException.ThrowIfNull(input);

        ICollection<EstablishmentDataTransferObject> dtoList =
            input as ICollection<EstablishmentDataTransferObject> ?? [.. input];

        int count = dtoList.Count;

        ArrayPool<Establishment> pool = ArrayPool<Establishment>.Shared;
        Establishment[] buffer = pool.Rent(count);

        int index = 0;

        try
        {
            foreach (EstablishmentDataTransferObject? dto in dtoList)
            {
                buffer[index++] = _establishmentMapper.Map(dto);
            }

            Establishment[] result = new Establishment[index];
            Array.Copy(buffer, result, index);

            return result;
        }
        finally
        {
            Array.Clear(buffer, 0, index);
            pool.Return(buffer);
        }
    }
}
