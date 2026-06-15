using System.Diagnostics.CodeAnalysis;
using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.UseCases.TestDoubles;

[ExcludeFromCodeCoverage]
public static class EstablishmentSearchResultTestDouble
{
    private static int FakeUrn(Faker faker) => faker.Random.Int(10000, 999999);
    private static string FakeName(Faker faker) => faker.Company.CompanyName();

    public static EstablishmentSearchResult Fake()
    {
        // Instantiate a Bogus faker for generating realistic fake data
        Faker faker = new();

        int urn = FakeUrn(faker);
        string name = FakeName(faker);

        return new(urn, name);
    }
}
