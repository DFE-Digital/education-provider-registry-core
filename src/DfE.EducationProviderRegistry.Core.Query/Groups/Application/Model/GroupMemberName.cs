namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupMemberName
{
    public GroupMemberName(string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        FullName = fullName;
    }

    public string FullName { get; }
}
