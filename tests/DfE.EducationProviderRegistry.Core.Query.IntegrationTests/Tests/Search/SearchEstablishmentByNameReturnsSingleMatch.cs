using System.Diagnostics;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
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
        builder.AddDefaultSearchConfiguration();
    }

    [Fact]
    public async Task Returns_Single_Result_Mapped()
    {
        // arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        const string searchTerm = "TEST";

        SearchableEstablishments searchedEstablishments =
            await SearchEstablishmentFactory.CreateManyAsync(
                totalToCreate: 100_000,
                searchTerm: searchTerm,
                matches: SearchByNameMatchTerms.Create(searchTerm, matchCount: 1),
                ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchKeywords(searchTerm)
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

        SearchResponseAssertions.AssertMatches(
            searchedEstablishments.Matches.Single(),
            result);
    }
}
