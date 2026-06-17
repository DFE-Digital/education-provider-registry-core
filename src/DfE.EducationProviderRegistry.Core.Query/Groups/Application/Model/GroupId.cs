namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupId
{
    public GroupId(string groupId)
    {
        // TODO validation on GroupId?
        ArgumentException.ThrowIfNullOrWhiteSpace(groupId);
        Value = groupId.Trim();
    }

    public string Value { get; }

    public override string ToString() => Value;
}
