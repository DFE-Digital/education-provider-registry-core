using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search;

public sealed class SearchEstablishmentByNameReturnsSingleMatch : UseCaseIntegrationTestBase
{
    public SearchEstablishmentByNameReturnsSingleMatch(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSearch(configuration);
    }

    protected override void ConfigureApplicationConfiguration(IConfigurationBuilder builder)
    {
        IndexedFieldConfiguration fieldBehaviour =
            IndexedFieldConfigurationBuilder.Create()
                .WithName("field")
                .WithChainingPredicate("AND")
                .WithBehaviour("behaviour", "AND")
                .Build();

        SearchConfigurationBuilder
            .Create()
            .WithBehaviourForSearchTerm(term: "term-1", [fieldBehaviour]);
    }

    [Fact]
    public async Task Returns_Single_Result_Mapped()
    {
        // arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        const string searchTerm = "TEST";

        SearchableEstablishmentsResponse searchedEstablishments =
            await SearchEstablishmentFactory.CreateManyAsync(
                totalToCreate: 100_000,
                searchTerm: searchTerm,
                matches: SearchByNameTerms.Create(searchTerm, matchCount: 1),
                ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm("what", searchTerm)
                .Build();

        // act
        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(request);

        // assert
        Assert.NotNull(response);
        Assert.Null(response.ErrorMessage);

        Assert.NotNull(response.Model);
        Assert.Equal(1, response.Model.TotalNumberOfResults);
        Assert.NotNull(response.Model.EstablishmentResults);

        EstablishmentSearchResult result =
            Assert.Single(
                response.Model.EstablishmentResults.EstablishmentCollection);

        SearchResponseAssertions.AssertMapped(
            searchedEstablishments.SearchTermMatches.Single(),
            result);
    }
}
