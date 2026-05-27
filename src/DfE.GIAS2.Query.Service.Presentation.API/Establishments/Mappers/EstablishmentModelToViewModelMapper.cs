using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.GIAS2.Query.Service.Core.Establishments.Application.Model;
using DfE.GIAS2.Query.Service.Presentation.API.Establishments.ViewModels;

namespace DfE.GIAS2.Query.Service.Presentation.API.Establishments.Mappers;

/// <summary>
/// Maps an <see cref="Establishment"/> domain model into a dynamic view model
/// suitable for API responses.
/// 
/// This mapper constructs a strongly typed <see cref="EstablishmentViewModel"/>.
/// </summary>
public sealed class EstablishmentModelToViewModelMapper
    : IMapper<Establishment, object?>
{
    /// <summary>
    /// Maps an <see cref="Establishment"/> domain model into a dynamic view model.
    /// </summary>
    /// <param name="input">The establishment domain model to map.</param>
    /// <returns>
    /// A dynamic object representing the establishment, shaped according to
    /// the active field selection rules.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="input"/> is <c>null</c>.
    /// </exception>
    public object Map(Establishment input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return new EstablishmentViewModel()
        {
            URN = input.Identifier.Urn,
        };
    }
}
