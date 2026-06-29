namespace DfE.EducationProviderRegistry.Core.Query.Shared;

// TODO: Remove this record and replace with SiteAddress
public sealed record Address(
    string Street,
    string Town,
    string County,
    string Postcode);

public sealed record SiteAddressModel(
    string Name,
    string AddressLine1,
    string AddressLine2,
    string Town,
    string County,
    string Postcode);
