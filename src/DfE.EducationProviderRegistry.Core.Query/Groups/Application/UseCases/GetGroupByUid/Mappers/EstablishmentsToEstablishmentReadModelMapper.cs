using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Response;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.UseCases.GetGroupByUid.Mappers;

internal sealed class EstablishmentsToEstablishmentReadModelMapper : IMapper<IEnumerable<Establishment>, IReadOnlyCollection<EstablishmentReadModel>>
{
    public IReadOnlyCollection<EstablishmentReadModel> Map(IEnumerable<Establishment> input)
    {
        return input
            .OrderBy((establishment) => establishment.Name)
            .Select((ordered) => new EstablishmentReadModel()
            {
                Name = ordered.Name.Value,
                Urn = ordered.Urn.Value
            })
            .ToArray();
    }
}
