using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Infrastructure;

internal sealed class GroupEstablishmentAggregateToEstablishmentsMapper : IMapper<IEnumerable<GroupEstablishmentAggregate>, IReadOnlyCollection<Establishment>>
{
    public IReadOnlyCollection<Establishment> Map(IEnumerable<GroupEstablishmentAggregate> input)
    {
        if (input is null)
        {
            return [];
        }

        return input.Select(CreateEstablishment).ToArray();
    }

    private static Establishment CreateEstablishment(GroupEstablishmentAggregate aggregate)
    {

        UniqueReferenceNumber urn = new(aggregate.EstablishmentUrn);

        Name name = new(aggregate.EstablishmentName);

        return new Establishment(urn, name);
    }
}
