namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record GroupTypeCode
{
    public string Value { get; }

    public GroupTypeCode(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public override string ToString() => Value;
}
