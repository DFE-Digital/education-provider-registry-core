
using DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.Persistence.Mappers.TestDoubles.StubBuilders;

public sealed class EstablishmentReadModelBuilder
{
    private string _urn = "100000";

    public EstablishmentReadModelBuilder WithUrn(string urn)
    {
        _urn = urn;
        return this;
    }

    public Establishment Build()
    {
        EstablishmentIdentifier identifier = new(_urn);
        return new Establishment(identifier);
    }

    public IReadOnlyCollection<Establishment> BuildMany(int count)
    {
        List<Establishment> establishments = [];

        for (int i = 0; i < count; i++)
        {
            string urn = $"10000{i}";
            EstablishmentIdentifier identifier = new(urn);
            establishments.Add(
                Establishment.Create(identifier));
        }

        return establishments;
    }
}
