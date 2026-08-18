using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search;

public sealed class SearchEstablishmentByNameReturnsExactMatch : UseCaseIntegrationTestBase
{
    public SearchEstablishmentByNameReturnsExactMatch(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSearch(configuration);
    }

    protected override void ConfigureApplicationConfiguration(IConfigurationBuilder builder)
    {
        builder
            .StubFilterOptions()
            .StubSearchCriteriaOptions();

        builder.AddSearchConfiguration(
            (
                termKey: "term-1",
                fieldsConfigure: [
                    (builder) =>
                        builder
                            .WithFieldName<Establishment>("Name")
                            .WithExactMatchBehaviour()
                            .Build()
                ]
            ));
    }

    [Theory]
    [InlineData("single school match", 1)]
    [InlineData("schools", 7)]
    [InlineData("CamelCase School", 7)]
    public async Task Returns_Exact_Matches(string searchTerm, int totalExactMatches)
    {
        // arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchByNameTerms matchTerms = new([.. Enumerable.Range(1, totalExactMatches).Select(t => searchTerm)]);

        SearchableEstablishmentsResponse searchedEstablishments =
            await SearchEstablishmentFactory.CreateManyAsync(totalToCreate: 100_000, matchTerms, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(key: "term-1", term: searchTerm)
                .Build();

        // act
        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(request);

        // assert
        Assert.NotNull(response);
        Assert.Null(response.ErrorMessage);

        Assert.NotNull(response.Model);
        Assert.Equal(totalExactMatches, response.Model.TotalNumberOfResults);
        Assert.NotNull(response.Model.EstablishmentResults);
        Assert.Equal(totalExactMatches, response.Model.EstablishmentResults.EstablishmentCollection.Count);

        for (int index = 0; index < searchedEstablishments.SearchTermMatches.Count; index++)
        {
            EstablishmentSearchResult actualEstablishment = response.Model.EstablishmentResults.EstablishmentCollection.ToList()[index];

            SearchResponseAssertions.AssertMapped(
                expected: searchedEstablishments.SearchTermMatches.Single(t => t.Urn == actualEstablishment.Urn.Value),
                actual: actualEstablishment);
        }
    }
}
