using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared.TestDoubles;

internal static class AddressTestDoubles
{
    internal static Address Stub() =>
        Create(
            street: "123 Example Street",
            town: "Testville",
            county: "Testshire",
            postcode: "TE5 7ST");


    internal static Address Generate()
    {
        Faker faker = new();

        return Create(
            street: faker.Address.StreetName(),
            town: faker.Address.City(),
            county: faker.Address.County(),
            postcode: faker.Address.ZipCode());
    }

    internal static Address Create(
        string? street,
        string? town = null,
        string? county = null,
        string? postcode = null)
    {
        return new(
            street!,
            town!,
            county!,
            postcode!);
    }
}
