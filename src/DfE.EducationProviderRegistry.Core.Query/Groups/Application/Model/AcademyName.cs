namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record AcademyName
{
    public AcademyName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    public string Name { get; }
}
