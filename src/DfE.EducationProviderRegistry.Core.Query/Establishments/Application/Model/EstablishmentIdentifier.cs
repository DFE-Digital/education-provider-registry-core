using System.Text.RegularExpressions;

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
public sealed partial record EstablishmentIdentifier
{
    /// <summary>
    /// Gets the establishment's URN (Unique Reference Number).
    /// Guaranteed to be valid according to the defined URN pattern.
    /// </summary>
    public string Urn { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EstablishmentIdentifier"/>
    /// record using the specified <paramref name="urn"/>.
    /// </summary>
    /// <param name="urn">
    /// The URN value to assign. Must be either <c>UNDEFINED</c> or a
    /// 5–7 digit numeric string.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="urn"/> does not match the required format.
    /// </exception>
    public EstablishmentIdentifier(string urn)
    {
        if (!UrnValidation().IsMatch(urn))
            throw new ArgumentException(
                "URN must be a valid 5–7 digit numeric value or 'UNDEFINED'.",
                nameof(urn));

        Urn = urn;
    }

    /// <summary>
    /// Returns the URN as a string.
    /// </summary>
    /// <returns>
    /// The URN value represented by this identifier.
    /// </returns>
    public override string ToString() => Urn;

    /// <summary>
    /// The regular expression pattern used to validate URN values.
    /// Accepts either <c>UNDEFINED</c> or a 5–7 digit numeric string.
    /// </summary>
    private const string UrnPattern = @"^UNDEFINED|\d{5,7}$";

    /// <summary>
    /// Creates a compiled regular expression used to validate URN values.
    /// This method is generated at compile time for optimal performance.
    /// </summary>
    private static Regex UrnValidation() => ValidateUrn();

    /// <summary>
    /// Source‑generated regular expression for URN validation.
    /// </summary>
    [GeneratedRegex(UrnPattern, RegexOptions.Compiled)]
    private static partial Regex ValidateUrn();
}
