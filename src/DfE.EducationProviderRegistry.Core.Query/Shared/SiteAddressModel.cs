namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public sealed record SiteAddressModel(
    string? Name,
    string? AddressLine1,
    string? AddressLine2,
    string? Town,
    string? County,
    string? Postcode)
{
    public override string ToString()
    {
        return string.Join(
            ", ",
            new[]
            {
                Name,
                AddressLine1,
                AddressLine2,
                Town,
                County,
                Postcode
            }
            .Where(static x =>
                !string.IsNullOrWhiteSpace(x)));
    }
}
