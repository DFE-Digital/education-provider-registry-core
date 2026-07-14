using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observor;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Sort;
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

    [Fact]
    public async Task Returns_Results()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;

        const string searchTerm = "TEST";
        const int matchingEstablishmentCount = 50;

        IReadOnlyCollection<Establishment> searchableEstablishments =
            await CreateSearchedEstablishment.CreateManyAsync(
                totalToCreate: 100_000,
                matchingSearchTermCount: matchingEstablishmentCount,
                searchTerm: searchTerm,
                ct);

        await QueryObservationHandler.StartAsync(ct);

        UseCaseResponse<SearchResponse> response =
            await ExecuteUseCase<SearchRequest, SearchResponse>(
                new SearchRequest(
                    searchIndexKey: "STUB_SEARCH_INDEX_KEY",
                    searchKeywords: searchTerm,
                    new SortOrder(sortField: "UNDEFINED", "asc",
                    validSortFields: ["UNDEFINED"])));


        PostgresQueryObservation observation =
            await QueryObservationHandler.GetObservationsAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(response);
        Assert.Null(response.ErrorMessage);
        Assert.NotNull(response.Model);
        Assert.Equal(matchingEstablishmentCount, response.Model.TotalNumberOfResults);
    }
}
