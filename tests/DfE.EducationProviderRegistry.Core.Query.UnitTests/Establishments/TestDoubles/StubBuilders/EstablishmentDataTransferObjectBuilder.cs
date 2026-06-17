using DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Establishments.TestDoubles.StubBuilders;

public sealed class EstablishmentDtoBuilder
{
    private string _urn = "100000";

    public EstablishmentDtoBuilder WithUrn(string urn)
    {
        _urn = urn;
        return this;
    }

    public EstablishmentDto Build()
    {
        return new EstablishmentDto
        {
            URN = _urn
        };
    }

#pragma warning disable CA1822 // Mark members as static
    public IReadOnlyCollection<EstablishmentDto> BuildMany(int count)
#pragma warning restore CA1822 // Mark members as static
    {
        List<EstablishmentDto> list = new(count);

        for (int i = 0; i < count; i++)
        {
            string generatedUrn = (100000 + i).ToString();

            list.Add(
                new EstablishmentDto
                {
                    URN = generatedUrn
                });
        }

        return list.AsReadOnly();
    }
}
