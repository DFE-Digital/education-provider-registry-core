using System.Diagnostics.CodeAnalysis;
using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search;

[ExcludeFromCodeCoverage]
public static class SearchAggregateResultTestDouble
{
    private static readonly Faker _faker = new();

    public static SearchAggregateResult Fake()
    {
        ProviderIdentifier providerIdentifier =
            new(_faker.Random.Int(10000, 99999).ToString());

        Name name =
            new(_faker.Company.CompanyName());

        SearchAddress address =
            new(_faker.Address.FullAddress());

        SearchType type =
            SearchType.Create("Academy", 1);

        GroupDetail group =
            GroupDetail.Create("Mock Trust", "TRUST001");

        SearchLocalAuthority localAuthority =
            SearchLocalAuthority.Create("Test LA");

        SearchCategory providerCategory = new("Establishment");

        int academyCount = 10;

        return SearchAggregateResult.Create(
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

    public static SearchAggregateResult WithProviderIdentifier(string urn) =>
        SearchAggregateResult.Create(
            new ProviderIdentifier(urn),
            new Name("Test School"),
            new SearchAddress(
                "123 Street, Town, County, PC1 1AA"
            ),
            SearchType.Create("Academy", 1),
            GroupDetail.Create("Mock Trust", "TRUST001"),
            SearchLocalAuthority.Create("Test LA"),
            new SearchCategory("Establishment"),
            academyCount: 10);
}
