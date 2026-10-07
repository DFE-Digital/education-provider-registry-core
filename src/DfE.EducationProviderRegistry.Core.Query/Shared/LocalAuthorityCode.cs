namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public readonly record struct LocalAuthorityCode
{
    public LocalAuthorityCode(int code)
    {
        Validate(code);
        Value = code;
    }

    public static LocalAuthorityCode Parse(string code)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        if (!int.TryParse(code, out int parsedCode))
        {
            throw new ArgumentException("Local Authority code must be a valid integer.", nameof(code));
        }

        Validate(parsedCode);
        return new LocalAuthorityCode(parsedCode);
    }

    // TODO validate rules on LocalAuthorityCodes
    private static void Validate(int code)
    {
        if (code < 0 || code > 999)
        {
            throw new ArgumentOutOfRangeException(nameof(code), "Local Authority code must be between 0 and 999.");
        }
    }

    public int Value { get; }
}
