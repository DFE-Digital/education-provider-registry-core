using System.Diagnostics.CodeAnalysis;
using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles;

[ExcludeFromCodeCoverage]
public static class EstablishmentSearchResultTestDouble
{
    private static readonly Faker _faker = new();

    public static EstablishmentSearchResult Create()
    {
        UniqueReferenceNumber urn =
            new(_faker.Random.Int(10000, 99999).ToString());

        Name name =
            new(_faker.Company.CompanyName());

        SiteAddressModel address =
            new(
                Name: string.Empty,
                AddressLine1: _faker.Address.StreetAddress(),
                AddressLine2: _faker.Address.SecondaryAddress(),
                Town: _faker.Address.City(),
                County: _faker.Address.County(),
                Postcode: _faker.Address.ZipCode());

        EstablishmentType type =
            EstablishmentType.Create("Academy");

        GroupDetail group =
            GroupDetail.Create("Mock Trust", "TRUST001");

        LocalAuthority localAuthority =
            LocalAuthority.Create("Test LA", "LA001");

        return EstablishmentSearchResult.Create(
            urn,
            name,
            address,
            type,
            group,
            localAuthority);
    }

    public static EstablishmentSearchResult WithUrn(string urn) =>
        EstablishmentSearchResult.Create(
            new UniqueReferenceNumber(urn),
            new Name("Test School"),
            new SiteAddressModel(
                Name: string.Empty,
                AddressLine1: "123 Street",
                AddressLine2: string.Empty,
                Town: "Town",
                County: "County",
                Postcode: "PC1 1AA"
            ),
            EstablishmentType.Create("Academy"),
            GroupDetail.Create("Mock Trust", "TRUST001"),
            LocalAuthority.Create("Test LA", "LA001"));
}
