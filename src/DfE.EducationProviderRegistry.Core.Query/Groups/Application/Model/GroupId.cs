namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupId
{
    public GroupId(string groupId)
    {
        // TODO validation on GroupId?
        if (string.IsNullOrWhiteSpace(groupId))
        {
            throw new InvalidGroupIdentifierException(groupId);
        }

        Value = groupId.Trim();
    }

    public string Value { get; }

    public override string ToString() => Value;
}
