using System.Text.RegularExpressions;

namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public partial record ProviderIdentifier
{
    public ProviderIdentifier(string identifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

        string normalisedIdentifier = identifier.Trim();

        if (!ProviderIdentifierValidation().IsMatch((normalisedIdentifier)))
        {
            throw new ArgumentException(
                $"Invalid provider identifer format: '{identifier}'.", paramName: nameof(identifier));
        }

        Value = normalisedIdentifier;
    }

    public string Value { get; }

    public override string ToString() => Value;

    /// <summary>
    /// The regular expression pattern used to validate provider identifier values.
    /// Accepts either <c>UNDEFINED</c> or a 4–7 digit numeric string.
    /// </summary>
    private const string ProviderIdentifierPattern = @"^\d{4,7}$";

    /// <summary>
    /// Creates a compiled regular expression used to validate provider identifier values.
    /// This method is generated at compile time for optimal performance.
    /// </summary>
    private static Regex ProviderIdentifierValidation () => ValidateProviderIdentifier();

    /// <summary>
    /// Source‑generated regular expression for provider identifier validation.
    /// </summary>
    [GeneratedRegex(ProviderIdentifierPattern, RegexOptions.Compiled)]
    private static partial Regex ValidateProviderIdentifier();
}
