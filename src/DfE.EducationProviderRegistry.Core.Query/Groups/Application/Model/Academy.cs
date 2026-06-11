namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Academy
{
    public Academy(AcademyIdentifier id, AcademyName name)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(name);
        Identifier = id;
        Name = name;
    }

    public AcademyIdentifier Identifier { get; }
    public AcademyName Name { get; }
}
