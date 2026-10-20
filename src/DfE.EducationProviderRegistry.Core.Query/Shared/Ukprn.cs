namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public sealed record class Ukprn
{
    public Ukprn(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }
}
