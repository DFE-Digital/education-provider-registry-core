using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;

public sealed class EstablishmentDataTransferObjectBuilder
{
    private string _urn = "100000";

    public EstablishmentDataTransferObjectBuilder WithUrn(string urn)
    {
        _urn = urn;
        return this;
    }

    public EstablishmentDataTransferObject Build()
    {
        return new EstablishmentDataTransferObject
        {
            URN = _urn
        };
    }

#pragma warning disable CA1822 // Mark members as static
    public IReadOnlyCollection<EstablishmentDataTransferObject> BuildMany(int count)
#pragma warning restore CA1822 // Mark members as static
    {
        List<EstablishmentDataTransferObject> list = new(count);

        for (int i = 0; i < count; i++)
        {
            string generatedUrn = (100000 + i).ToString();

            list.Add(
                new EstablishmentDataTransferObject
                {
                    URN = generatedUrn
                });
        }

        return list.AsReadOnly();
    }
}
