using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

internal static class AddressTestDoubles
{
    internal static SiteAddressModel Stub() =>
        Create(
            AddressLine1: "123 Example Street",
            Town: "Testville",
            County: "Testshire",
            Postcode: "TE5 7ST");


    internal static SiteAddressModel Generate()
    {
        Faker faker = new();

        return Create(
            AddressLine1: faker.Address.StreetName(),
            AddressLine2: faker.Address.SecondaryAddress(),
            Town: faker.Address.City(),
            County: faker.Address.County(),
            Postcode: faker.Address.ZipCode());
    }

    internal static SiteAddressModel Create(
        string? AddressLine1,
        string? AddressLine2 = null,
        string? Town = null,
        string? County = null,
        string? Postcode = null)
    {
        return new(
            Name: string.Empty,
            AddressLine1: AddressLine1!,
            AddressLine2: AddressLine2!,
            Town: Town!,
            County: County!,
            Postcode: Postcode!);
    }
}
