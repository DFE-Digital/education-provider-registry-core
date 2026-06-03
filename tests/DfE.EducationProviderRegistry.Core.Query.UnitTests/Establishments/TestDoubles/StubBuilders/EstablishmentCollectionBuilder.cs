using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;


internal sealed class EstablishmentCollectionBuilder
{
    private int _count = 10;

    public EstablishmentCollectionBuilder WithCount(int count)
    {
        _count = count;
        return this;
    }

    public IReadOnlyCollection<Establishment> Build()
    {
        List<Establishment> establishmentList = new(_count);
        HashSet<string> urns = GenerateUniqueUrns(_count);

        foreach (var urn in urns)
        {
            establishmentList.Add(
                new Establishment(
                    new EstablishmentIdentifier(urn)));
        }

        return establishmentList.AsReadOnly();
    }

    private static HashSet<string> GenerateUniqueUrns(int count)
    {
        HashSet<string> urns = [];
        Random random = new();

        while (urns.Count < count)
        {
            int number = random.Next(100000, 999999); // 6 digits
            urns.Add(number.ToString());
        }

        return urns;
    }
}

