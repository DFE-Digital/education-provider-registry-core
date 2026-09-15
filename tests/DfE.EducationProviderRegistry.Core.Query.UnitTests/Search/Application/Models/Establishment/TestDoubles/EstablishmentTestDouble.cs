using System.Diagnostics.CodeAnalysis;
using Bogus;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Shared;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Application.Models.Establishment.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchAggregateTestDouble
{
    public static ProviderIdentifier ValidIdentifier => new("12345");

    public static Name ValidName => new("Test School");

    public static SearchAddress ValidAddress =>
        new("123 Example Street, Testville, Testshire, TE5 7ST");

    public static SearchType ValidType =>
        SearchType.Create("Academy", 1);

    public static GroupDetail ValidGroup =>
        GroupDetail.Create("Mock Trust", "TRUST001");

    public static SearchLocalAuthority ValidLocalAuthority =>
        SearchLocalAuthority.Create("Test LA");

    public static SearchCategory ProviderCategory =>
        SearchCategory.Create("Establishment");

    public static int FakeAcademyCount(Faker faker) =>
        faker.Random.Int(1, 100);
}

