namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public readonly record struct GroupUID
{
    public GroupUID(long groupId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(groupId);
        Value = groupId;
    }

    public long Value { get; }

    public static bool TryCreate(string input, out GroupUID result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        if (!int.TryParse(input.Trim(), out int parsed))
        {
            return false;
        }


        if (parsed <= 0)
        {
            return false;
        }

        result = new GroupUID(parsed);
        return true;
    }
}
