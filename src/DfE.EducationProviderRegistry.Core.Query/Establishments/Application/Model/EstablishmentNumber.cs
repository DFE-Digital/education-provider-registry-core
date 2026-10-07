namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Application.Model;

public readonly record struct EstablishmentNumber
{
    public EstablishmentNumber(int value)
    {
        if (value < 0 || value > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Establishment number must be between 0 and 9999.");
        }

        Value = value;
    }

    public static EstablishmentNumber Parse(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!int.TryParse(value, out int parsedValue))
        {
            throw new ArgumentException("Establishment number must be a valid integer.", nameof(value));
        }

        return new EstablishmentNumber(parsedValue);
    }
    public int Value { get; }
}
