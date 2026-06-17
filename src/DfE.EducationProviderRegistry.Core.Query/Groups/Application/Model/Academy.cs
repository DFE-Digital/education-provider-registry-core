namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record Academy
{
    public Academy(AcademyId id, AcademyName name)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(name);
        Id = id;
        Name = name;
    }

    public AcademyId Id { get; }
    public AcademyName Name { get; }
}
