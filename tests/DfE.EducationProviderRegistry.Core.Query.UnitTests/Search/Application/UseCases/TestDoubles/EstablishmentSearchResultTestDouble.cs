using System.Diagnostics.CodeAnalysis;
using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
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

    public static EstablishmentSearchResult Fake()
    {
        Faker faker = new();

        return EstablishmentSearchResult.Create(
            urn: new UniqueReferenceNumber(FakeUrn(faker).ToString()),
            name: new Name(FakeName(faker)),
            address: new Address(
                Street: FakeStreet(faker),
                Town: FakeTown(faker),
                County: FakeCounty(faker),
                Postcode: FakePostcode(faker)),
            type: EstablishmentType.Create("Academy"),
            group: GroupDetail.Create("Mock Group Id", "Mock Trust", "TRUST001"),
            localAuthority: LocalAuthority.Create("Test LA", "LA001")
        );
    }
}
