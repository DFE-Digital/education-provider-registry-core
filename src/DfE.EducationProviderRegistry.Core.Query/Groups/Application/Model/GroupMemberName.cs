namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupMemberName
{
    public GroupMemberName(string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        FullName = fullName.Trim();
    }

    public string FullName { get; }

    public bool Equals(GroupMemberName? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(
            FullName,
            other.FullName,
            StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(FullName);
    }
}
