using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Fields;

public sealed class SearchUseCaseCountyTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    public SearchUseCaseCountyTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            (field) =>
                field.WithFieldName(nameof(SearchAggregate.County))
                    .AppendExactMatchBehaviour()
                    .AppendContainsMatchBehaviour()
                    .AppendStartsWithMatchBehaviour());
    }

    [Fact]
    public async Task Search_ByCounty_Returns_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithCounty("a").Build(),
            SearchAggregateBuilder.Create().WithCounty("aBC").Build(),
            SearchAggregateBuilder.Create().WithCounty("bac").Build(),
            SearchAggregateBuilder.Create().WithCounty("d").Build(),
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "a")
                .Build();

        // Act Assert
        UseCaseResponse<SearchResponse> response =
            await ExecuteSuccessfulSearchAndAssertSearchAsync(
                request,
                expectedInResults: [seed[0], seed[1], seed[2]],
                notExpectedInResults: [seed[3]]);

        Assert.NotNull(response);
        Assert.True(response.SuccessfulRequest);
    }

    [Fact]
    public async Task Search_ByCounty_No_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithCounty("a").Build(),
            SearchAggregateBuilder.Create().WithCounty("aBC").Build(),
            SearchAggregateBuilder.Create().WithCounty("bac").Build(),
            SearchAggregateBuilder.Create().WithCounty("c").Build(),
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "NOTHING")
                .Build();

        // Act Assert
        UseCaseResponse<SearchResponse> response =
            await ExecuteSuccessfulSearchAndAssertSearchAsync(
                request,
                expectedInResults: [],
                notExpectedInResults: seed);

        Assert.NotNull(response);
        Assert.Empty(response.Model.SearchProviderResults!.SearchResultCollection);
    }
}
