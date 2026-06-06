
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
}
