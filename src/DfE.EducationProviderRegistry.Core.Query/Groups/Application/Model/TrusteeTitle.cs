namespace DfE.EducationProviderRegistry.Core.Query.Groups.Application.Model;

public sealed record TrusteeTitle
{
    public string Value { get; }
    public TrusteeTitleType Type { get; }

    public TrusteeTitle(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Value = title.Trim();
        Type = Normalise(Value);
    }


    private static TrusteeTitleType Normalise(string value)
    {
        string normalised = value.Trim().ToLowerInvariant();

        if (normalised.Contains("chair"))
        {
            return TrusteeTitleType.Chair;
        }

        if (normalised.Contains("cfo"))
        {
            return TrusteeTitleType.CFO;
        }

        if (normalised.Contains("accounting officer"))
        {
            return TrusteeTitleType.AccountingOfficer;
        }

        return TrusteeTitleType.Other;
    }

}

public enum TrusteeTitleType
{
    Other = 0,
    Chair,
    CFO,
    AccountingOfficer
}
