using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

/// <summary>
/// Represents a strongly‑typed identifier for an establishment within the
/// Education Provider Registry. This value object encapsulates the URN
/// (Unique Reference Number) and ensures it conforms to the expected format.
/// </summary>
/// <remarks>
/// A valid URN is either the literal string <c>UNDEFINED</c> or a numeric
/// value consisting of 5 to 7 digits. Validation is performed during
/// construction, guaranteeing that all instances represent well‑formed
/// identifiers.
/// </remarks>
public sealed partial record EstablishmentUrn
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentIdentifier"/>
    /// record using the specified <paramref name="urn"/>.
    /// </summary>
    /// <param name="urn">
    /// The URN value to assign. Must be a 5–7 digit numeric string.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="urn"/> does not match the required format.
    /// </exception>
    public EstablishmentIdentifier(UniqueReferenceNumber urn)
    {
        ArgumentNullException.ThrowIfNull(urn);
        Value = urn.Value;
    }

    /// <summary>
    /// Gets the establishment's URN (Unique Reference Number).
    /// Guaranteed to be valid according to the defined URN pattern.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Returns the URN as a string.
    /// </summary>
    /// <returns>
    /// The URN value represented by this identifier.
    /// </returns>
    public override string ToString() => Value;


    public static EstablishmentIdentifier Create(string urn)
    {
        UniqueReferenceNumber validated = new(urn?.Trim() ?? null!);
        return new EstablishmentIdentifier(validated);
    }
}
