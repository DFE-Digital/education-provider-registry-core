namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record EstablishmentStatus
{
    private static readonly HashSet<string> Allowed =
    [
        "Open",
        "Closed"
    ];

    public string Value { get; }

    public EstablishmentStatus(string value)
    {
        if (!Allowed.Contains(value))
            throw new EstablishmentException("Invalid establishment status.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}
