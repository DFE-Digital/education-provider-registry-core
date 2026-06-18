namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public readonly record struct GroupUID
{
    public GroupUID(int groupId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(groupId);
        Value = groupId;
    }

    public int Value { get; }
}
