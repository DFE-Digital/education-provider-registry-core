using System.Diagnostics;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observer.Postgres;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search;

public sealed class SearchEstablishmentByNamePerformanceTests : UseCaseIntegrationTestBase
{
    public SearchEstablishmentByNamePerformanceTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddSearch(configuration);
    }

    protected override void ConfigureApplicationConfiguration(IConfigurationBuilder builder)
    {
        builder.AddDefaultSearchConfiguration();
    }


    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(5000)]
    public async Task Executes_Within_2_Seconds(int matches)
    {
        // arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        const string searchTerm = "TEST";

        SearchableEstablishments _ =
            await SearchEstablishmentFactory.CreateManyAsync(
                totalToCreate: 100_000,
                searchTerm: searchTerm,
                matches: SearchByNameMatchTerms.Create(searchTerm, matchCount: matches),
                ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchKeywords(searchTerm)
                .Build();

        await QueryCollector.StartAsync(ct);
        Stopwatch stopwatch = Stopwatch.StartNew();

        // act
        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(request);

        // assert
        stopwatch.Stop();
        PostgresQueries observations = await QueryCollector.GetObservationsAsync(ct);

        Assert.NotNull(response);
        Assert.True(response.SuccessfulRequest);
        Assert.NotEmpty(response.Model!.EstablishmentResults!.EstablishmentCollection);

        const double expectedTotalExecutionTimeMilliseconds = 2_000;

        Assert.True(
            stopwatch.ElapsedMilliseconds < expectedTotalExecutionTimeMilliseconds,
                $"search exceeded {expectedTotalExecutionTimeMilliseconds}ms and took {stopwatch.ElapsedMilliseconds}ms");

        SearchQueryObservations searchQueryObservations = new(observations);
        searchQueryObservations.AssertSearchPipelineExecuted(TimeSpan.FromMilliseconds(1000));
    }
}
