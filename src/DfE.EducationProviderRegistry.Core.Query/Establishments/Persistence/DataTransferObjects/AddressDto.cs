namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.DataTransferObjects;

public sealed class AddressDto
{
    public required string Street { get; init; }
    public required string Town { get; init; }
    public required string County { get; init; }
    public required string Postcode { get; init; }
}
