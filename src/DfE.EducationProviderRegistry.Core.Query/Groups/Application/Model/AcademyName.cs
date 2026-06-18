using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record AcademyName
{
    public AcademyName(string name)
    {
        Value = new Name(name);
    }

    public Name Value { get; }

    public override string ToString() => Value.ToString();
}
