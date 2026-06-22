using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record AcademyId
{
    public AcademyId(UniqueReferenceNumber urn)
    {
        ArgumentNullException.ThrowIfNull(urn);
        Value = urn.Value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
