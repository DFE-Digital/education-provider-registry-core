namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Academy
{
    public Academy(AcademyIdentifier id, AcademyName name)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(name);
        Id = id;
        Name = name;
    }

    public AcademyIdentifier Id { get; }
    public AcademyName Name { get; }
}
