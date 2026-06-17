using System.Text.RegularExpressions;

namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public partial record UniqueReferenceNumber
{
    public UniqueReferenceNumber(string urn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(urn);

        string normalisedUrn = urn.Trim();

        if (!UrnValidation().IsMatch((normalisedUrn)))
        {
            throw new ArgumentException($"Invalid URN format: '{urn}'.", paramName: nameof(urn));
        }

        Value = normalisedUrn;
    }

    public string Value { get; }

    public override string ToString() => Value;

    /// <summary>
    /// The regular expression pattern used to validate URN values.
    /// Accepts either <c>UNDEFINED</c> or a 5–7 digit numeric string.
    /// </summary>
    private const string UrnPattern = @"^\d{5,7}$";

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
