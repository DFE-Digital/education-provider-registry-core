namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public sealed record Ukprn
{
    public Ukprn(string? value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }
}
