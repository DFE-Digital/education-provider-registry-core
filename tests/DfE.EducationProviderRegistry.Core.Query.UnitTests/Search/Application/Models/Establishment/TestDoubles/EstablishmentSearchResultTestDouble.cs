using System.Diagnostics.CodeAnalysis;
using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles;

[ExcludeFromCodeCoverage]
public static class SearchResultTestDouble
{
    private static readonly Faker _faker = new();

    public static SearchProviderResult Create()
    {
        ProviderIdentifier providerIdentifier =
            new(_faker.Random.Int(10000, 99999).ToString());

        Name name =
            new(_faker.Company.CompanyName());

        SearchProviderAddress address =
            new(_faker.Address.FullAddress());

        SearchProviderType type =
            SearchProviderType.Create("Academy", 1);

        GroupDetail group =
            GroupDetail.Create("Mock Trust", "TRUST001");

        SearchProviderLocalAuthority localAuthority =
            SearchProviderLocalAuthority.Create("Test LA");

        SearchProviderCategory providerCategory = new("Establishment");

        int academyCount = 10;

        return SearchProviderResult.Create(
            providerIdentifier,
            name,
            address,
            type,
            group,
            localAuthority,
            providerCategory,
            academyCount
            );
    }

    public static SearchProviderResult WithProviderIdentifier(string urn) =>
        SearchProviderResult.Create(
            new ProviderIdentifier(urn),
            new Name("Test School"),
            new SearchProviderAddress(
                "123 Street, Town, County, PC1 1AA"
            ),
            SearchProviderType.Create("Academy", 1),
            GroupDetail.Create("Mock Trust", "TRUST001"),
            SearchProviderLocalAuthority.Create("Test LA"),
            new SearchProviderCategory("Establishment"),
            academyCount: 10);
}
