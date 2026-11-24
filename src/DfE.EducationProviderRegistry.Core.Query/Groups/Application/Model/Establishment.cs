using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Establishment
{
    public Establishment(
        UniqueReferenceNumber urn,
        Name name)
    {
        Urn = urn;
        Name = name;
    }

    public UniqueReferenceNumber Urn { get; }
    public Name Name { get; }
}
