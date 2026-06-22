namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public sealed record Address(
    string Street,
    string Town,
    string County,
    string Postcode);


public sealed record SiteAddress(
    string Name,
    string AddressLine1,
    string AddressLine2,
    string Town,
    string County,
    string Postcode);
