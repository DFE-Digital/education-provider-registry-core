namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupType
{
    public GroupType(string? groupType)
    {
        Value = groupType ?? string.Empty;
    }

    public string Value { get; }
}
