namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupType
{
    public GroupType(string? name, GroupTypeCode code)
    {
        ArgumentNullException.ThrowIfNull(code);
        Name = name ?? string.Empty;
        Code = code;
    }

    public string Name { get; }
    public GroupTypeCode Code { get; }
}
