namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public sealed record Name
{
    public Name(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Value = name.Trim();
    }

    public string Value { get; }

    public bool Equals(Name? other)
    {
        if (other is null)
        {
            return false;
        }

        return string.Equals(
            Value,
            other.Value,
            StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    }
}
