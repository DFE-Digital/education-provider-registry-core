using System.Diagnostics.CodeAnalysis;
using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchResultTestDouble
{
    private static int FakeUrn(Faker faker) => faker.Random.Int(10000, 999999);
    private static string FakeName(Faker faker) => faker.Company.CompanyName();
    private static int FakeAcademyCount(Faker faker) => faker.Random.Int(1, 100);

    public static SearchAggregateResult Fake()
    {
        Faker faker = new();

        return SearchAggregateResult.Create(
        uniqueIdentifier: new ProviderIdentifier(FakeUrn(faker).ToString()),
        name: new Name(FakeName(faker)),
        address: new SearchAddress(faker.Address.FullAddress()),
        type: SearchType.Create("Academy", 1),
        group: GroupDetail.Create("Mock Trust", "TRUST001"),
        localAuthority: SearchLocalAuthority.Create("Test LA"),
        providerCategory: new SearchCategory("Establishment"),
        academyCount: FakeAcademyCount(faker));
    }
}
