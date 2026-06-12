using System.Text.RegularExpressions;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed partial record EstablishmentUkprn
{
    public string Value { get; }

    public EstablishmentUkprn(string value)
    {
        if (!UkprnValidation().IsMatch(value))
            throw new EstablishmentException(
                "UKPRN must be an 8‑digit numeric value.",
                nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    private const string UkprnPattern = @"^\d{8}$";
    private static Regex UkprnValidation() => ValidateUkprn();

    [GeneratedRegex(UkprnPattern, RegexOptions.Compiled)]
    private static partial Regex ValidateUkprn();
}
