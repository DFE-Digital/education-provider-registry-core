using System.Text.RegularExpressions;

namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public sealed partial record GovernanceIdentifier
{
    public string Value { get; }

    public GovernanceIdentifier(string value)
    {
        if (!IdentifierValidation().IsMatch(value))
            throw new ArgumentException("Governance identifier must be 7 digits.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    private static Regex IdentifierValidation() => ValidateIdentifier();

    private const string UrnPattern = @"^\d{7}$";

    [GeneratedRegex(UrnPattern, RegexOptions.Compiled)]
    private static partial Regex ValidateIdentifier();
}
