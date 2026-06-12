namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record PhaseOfEducation
{
    private static readonly HashSet<string> AllowedValues =
    [
        "Primary",
        "Secondary",
        "Not applicable"
    ];

    public string Value { get; }

    public PhaseOfEducation(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new EstablishmentException(
                "Phase of education cannot be null or empty.",
                nameof(value));

        string normalised = value.Trim();

        if (!AllowedValues.Contains(normalised))
            throw new EstablishmentException(
                $"Invalid phase of education: '{value}'.",
                nameof(value));

        Value = normalised;
    }

    public override string ToString() => Value;
}
