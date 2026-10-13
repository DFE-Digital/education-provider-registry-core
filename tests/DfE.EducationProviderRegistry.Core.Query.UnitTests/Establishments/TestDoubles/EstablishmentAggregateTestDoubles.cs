using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles;

internal static class EstablishmentAggregateTestDoubles
{
    public static EstablishmentAggregate Stub()
    {
        return StubMany(1).Single();
    }

    public static IReadOnlyCollection<EstablishmentAggregate> StubMany(int count)
    {
        List<EstablishmentAggregate> list = new(count);

        for (int i = 0; i < count; i++)
        {
            list.Add(
                new EstablishmentAggregateBuilder()
                    .WithUrn((100000 + i).ToString())
                    .Build());
        }

        return list.AsReadOnly();
    }
}
