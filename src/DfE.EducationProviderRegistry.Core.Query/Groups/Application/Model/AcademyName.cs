namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record AcademyName
{
    public AcademyName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Value = name;
    }

    public string Value { get; }
}
