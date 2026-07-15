using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search;

public sealed class SearchEstablishmentByNameLimitsResults : UseCaseIntegrationTestBase
{
    public SearchEstablishmentByNameLimitsResults(IServiceProvider testServicesProvider) : base(testServicesProvider)
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
    public async Task Limits_Matches_To_50_Results()
    {
        // arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        const string searchTerm = "testlimit";

        SearchableEstablishments searchedEstablishments =
            await SearchEstablishmentFactory.CreateManyAsync(
                totalToCreate: 100_000,
                searchTerm: searchTerm,
                matches: SearchByNameMatchTerms.Create(searchTerm, 5000),
                ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchKeywords(searchTerm)
                .Build();

        // act
        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(request);

        // assert
        const int limit = 50;

        Assert.NotNull(response);
        Assert.Null(response.ErrorMessage);
        Assert.NotNull(response.Model);
        Assert.Equal(limit, response.Model.TotalNumberOfResults);
        Assert.Equal(limit, response.Model.EstablishmentResults!.EstablishmentCollection.Count);
    }
}
