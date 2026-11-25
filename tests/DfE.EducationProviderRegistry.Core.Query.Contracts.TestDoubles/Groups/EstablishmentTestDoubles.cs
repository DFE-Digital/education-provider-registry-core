using DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Groups;

public static class EstablishmentTestDoubles
{
    public static IReadOnlyCollection<Establishment> Create(int count = 1)
    {
        return Enumerable.Range(1, count)
            .Select(index => new Establishment(
                new UniqueReferenceNumber($"{100000 + index}"),
                new Name($"Establishment {index}")))
            .ToArray();
    }
}
