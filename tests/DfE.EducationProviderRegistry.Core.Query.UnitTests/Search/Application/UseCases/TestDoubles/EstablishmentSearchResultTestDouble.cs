using System.Diagnostics.CodeAnalysis;
using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EstablishmentSearchResultTestDouble
{
    private static int FakeUrn(Faker faker) => faker.Random.Int(10000, 999999);
    private static string FakeName(Faker faker) => faker.Company.CompanyName();
    private static string FakeStreet(Faker faker) => faker.Address.StreetAddress();
    private static string FakeTown(Faker faker) => faker.Address.City();
    private static string FakeCounty(Faker faker) => faker.Address.County();
    private static string FakePostcode(Faker faker) => faker.Address.ZipCode();

    public static SearchProviderResult Fake()
    {
        Faker faker = new();

        return SearchProviderResult.Create(
            uniqueIdentifier: new ProviderIdentifier(FakeUrn(faker).ToString()),
            name: new Name(FakeName(faker)),
            address: new SearchProviderAddress(faker.Address.FullAddress()),
            type: SearchProviderType.Create("Academy", 1),
            group: GroupDetail.Create("Mock Trust", "TRUST001"),
            localAuthority: SearchProviderLocalAuthority.Create("Test LA"),
            providerCategory: new SearchProviderCategory("Establishment")
        );
    }
}
