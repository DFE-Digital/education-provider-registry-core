using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Fields;

public sealed class SearchUseCaseProviderIdTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    public SearchUseCaseProviderIdTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            (field) =>
                field.WithFieldName(nameof(SearchAggregate.ProviderId))
                    .AppendContainsMatchBehaviour()
                    .AppendStartsWithMatchBehaviour());
    }

    [Fact]
    public async Task Search_ByProvderId_Returns_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithProviderId(9996).Build(),
            SearchAggregateBuilder.Create().WithProviderId(900009).Build(),
            SearchAggregateBuilder.Create().WithProviderId(10991).Build(),
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "99")
                .Build();

        // Act
        UseCaseResponse<SearchResponse> response =
            await ExecuteAndAssertSearchAsync(
                request,
                expectedInResults: [seed[0], seed[2]],
                notExpectedInResults: [seed[1]]);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Model);
        Assert.True(response.SuccessfulRequest);
    }

    [Fact]
    public async Task Search_ByProviderId_No_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithProviderId(100000).Build(),
            SearchAggregateBuilder.Create().WithProviderId(999999).Build(),
            SearchAggregateBuilder.Create().WithProviderId(123456).Build(),
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "NOTHING")
                .Build();

        // Act
        UseCaseResponse<SearchResponse> response =
            await ExecuteAndAssertSearchAsync(
                request,
                expectedInResults: [],
                notExpectedInResults: seed);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Model);
        Assert.True(response.SuccessfulRequest);
        Assert.Empty(response.Model.SearchProviderResults!.SearchResultCollection);
    }
}

