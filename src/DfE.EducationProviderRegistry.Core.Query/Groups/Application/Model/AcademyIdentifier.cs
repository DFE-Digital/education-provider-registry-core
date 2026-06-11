using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record AcademyIdentifier
{
    public AcademyIdentifier(UniqueReferenceNumber urn)
    {
        ArgumentNullException.ThrowIfNull(urn);
        Value = urn.Value;
    }

    public string Value { get; }
}
