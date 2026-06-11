namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupMemberIdentifier
{
    public GroupMemberIdentifier(string governorId)
    {
        // TODO validation on GovernorId?
        ArgumentException.ThrowIfNullOrWhiteSpace(governorId);
        Value = governorId;
    }

    public string Value { get; }

    public override string ToString() => Value;

    public static GroupMemberIdentifier Create(string governorId) => new(governorId);
}
