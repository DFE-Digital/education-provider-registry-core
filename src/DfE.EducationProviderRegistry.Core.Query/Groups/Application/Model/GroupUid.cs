namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public readonly record struct GroupUid
{
    public GroupUid(int groupId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(groupId);
        Value = groupId;
    }

    public int Value { get; }
}
