using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Fields;

public sealed class SearchUseCaseTownTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    public SearchUseCaseTownTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            (field) =>
                field.WithFieldName(nameof(SearchAggregate.Town))
                    .AppendContainsMatchBehaviour()
                    .AppendStartsWithMatchBehaviour());
    }

    [Fact]
    public async Task Search_ByTown_Returns_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithTown("a").Build(),
            SearchAggregateBuilder.Create().WithTown("bac").Build(),
            SearchAggregateBuilder.Create().WithTown("d").Build(),
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "a")
                .Build();

        // Act
        UseCaseResponse<SearchResponse> response =
            await ExecuteAndAssertSearchAsync(
                request,
                expectedInResults: [seed[0], seed[1]],
                notExpectedInResults: [seed[2]]);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Model);
        Assert.True(response.SuccessfulRequest);
    }

    [Fact]
    public async Task Search_ByTown_No_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithTown("a").Build(),
            SearchAggregateBuilder.Create().WithTown("bac").Build(),
            SearchAggregateBuilder.Create().WithTown("d").Build(),
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

