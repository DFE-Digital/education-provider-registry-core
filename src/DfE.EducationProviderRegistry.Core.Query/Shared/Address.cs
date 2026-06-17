namespace DfE.EducationProviderRegistry.Core.Query.Shared;

public sealed record Address(
    string Street,
    string Town,
    string County,
    string Postcode);
