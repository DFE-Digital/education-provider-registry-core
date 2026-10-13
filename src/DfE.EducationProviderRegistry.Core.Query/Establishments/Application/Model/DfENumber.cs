using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public sealed record class DfeNumber
{
    public DfeNumber(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        string[] parts = value.Split('/');

        if (parts.Length != 2)
        {
            throw new ArgumentException("DfE number must be in format 'XXX/YYYY'.", nameof(value));
        }

        _ = LocalAuthorityCode.Parse(parts[0]);
        _ = EstablishmentNumber.Parse(parts[1]);

        Value = value;
    }

    public string Value { get; }

    public static DfeNumber Create(LocalAuthorityCode laCode, EstablishmentNumber estabNumber)
    {
        string formattedDfENumber = $"{laCode.Value}/{estabNumber.Value}";
        return new DfeNumber(formattedDfENumber);
    }
}
