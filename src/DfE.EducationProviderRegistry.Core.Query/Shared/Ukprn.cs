namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public sealed record Ukprn
{
    public Ukprn(string? value)
    {
        Value = value ?? string.Empty;
    }

    public string Value { get; }

    public static Ukprn CreateNoValue() => new(string.Empty);
}
