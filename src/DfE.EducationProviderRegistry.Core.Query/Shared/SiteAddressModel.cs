namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public sealed record SiteAddressModel(
    string Name,
    string AddressLine1,
    string AddressLine2,
    string Town,
    string County,
    string Postcode);
