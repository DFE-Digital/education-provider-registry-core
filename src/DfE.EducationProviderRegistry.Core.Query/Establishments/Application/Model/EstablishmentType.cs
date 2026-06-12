namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record EstablishmentType
{
    private static readonly HashSet<string> AllowedValues =
    [
        "Academy",
        "Community school",
        "Foundation school",
        "Voluntary aided school",
        "British schools overseas",
        "Other"
    ];

    public string Value { get; }

    public EstablishmentType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new EstablishmentException(
                "Establishment type cannot be null or empty.",
                nameof(value));

        string normalised = value.Trim();

        if (!AllowedValues.Contains(normalised))
            throw new EstablishmentException(
                $"Invalid establishment type: '{value}'.",
                nameof(value));

        Value = normalised;
    }

    public override string ToString() => Value;
}
