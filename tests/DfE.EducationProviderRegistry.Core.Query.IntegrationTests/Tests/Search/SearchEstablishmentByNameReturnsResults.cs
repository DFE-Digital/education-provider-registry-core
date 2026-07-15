using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search;

public sealed class SearchEstablishmentByNameReturnsResults : UseCaseIntegrationTestBase
{
    public SearchEstablishmentByNameReturnsResults(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSearch(configuration);
    }

    protected override void ConfigureApplicationConfiguration(IConfigurationBuilder builder)
    {
        builder.AddDefaultSearchConfiguration();
    }

    [Theory]
    [MemberData(nameof(SearchScenarios))]
    public async Task Returns_Multiple_Results_Mapped(SearchScenario scenario)
    {
        // arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchableEstablishmentsResponse searchedEstablishments =
            await SearchEstablishmentFactory.CreateManyAsync(
                totalToCreate: 100_000,
                searchTerm: scenario.searchTerm,
                matches: scenario.matches,
                ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchKeywords(scenario.searchTerm)
                .Build();

        // act
        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(request);

        // assert
        Assert.NotNull(response);
        Assert.Null(response.ErrorMessage);
        Assert.NotNull(response.Model);
        Assert.Equal(scenario.matches.matchingNames.Count, response.Model.TotalNumberOfResults);

        IReadOnlyCollection<EstablishmentSearchResult> actualResults = response.Model.EstablishmentResults!.EstablishmentCollection;

        Assert.Equal(searchedEstablishments.SearchTermMatches.Count, actualResults.Count);

        foreach (Establishment matchedEstablishment in searchedEstablishments.SearchTermMatches)
        {
            EstablishmentSearchResult result =
                Assert.Single(actualResults, (estab) => estab.Urn.Value == matchedEstablishment.Urn);

            SearchResponseAssertions.AssertMapped(matchedEstablishment, result);
        }
    }

    public sealed record SearchScenario(string searchTerm, SearchByNameTerms matches);

    public static TheoryData<SearchScenario> SearchScenarios =>
    [
        new SearchScenario(
            "test",
            SearchByNameTerms.Create("test", 50)),

        // Case insensitive single-word term
        new SearchScenario(
            "Establishment",
            new([
                "establishment",
                "EsTaBlIshMeNT",
                "ESTABLISHMENT"
            ])),
        // Case insentive match multi word
        new SearchScenario(
            "The Establishment",
            new([
                "The Primary Establishment",
                "My establishment",
                "Establishment",
                "ESTABLISHMENT",
                "establishment"
            ])),
        // Special character - '
        new SearchScenario(
            "o'connor",
            new(
            [
                "O'Connor Academy",
                "St O'Connor's School"
            ])),
        // Special character - @
        new SearchScenario(
            "@",
            new(
            [
                "@ Academy",
                "Coll@ge"
            ]))
    ];
}
