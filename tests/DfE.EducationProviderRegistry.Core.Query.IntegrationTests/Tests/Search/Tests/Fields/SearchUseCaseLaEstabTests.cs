using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Fields;

public sealed class SearchUseCaseLaEstabTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    public SearchUseCaseLaEstabTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            (field) =>
                field.WithFieldName(nameof(SearchAggregate.LaEstab))
                    .AppendExactMatchBehaviour()
                    .AppendContainsMatchBehaviour()
                    .AppendStartsWithMatchBehaviour());
    }

    [Fact]
    public async Task Search_ByLaEstab_Returns_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithLaEstab("1").Build(), // exact
            SearchAggregateBuilder.Create().WithLaEstab("123").Build(), // contains
            SearchAggregateBuilder.Create().WithLaEstab("10").Build(), // startsWith
            SearchAggregateBuilder.Create().WithLaEstab("2").Build(), // noMatch
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "1")
                .Build();

        // Act
        UseCaseResponse<SearchResponse> response =
            await ExecuteSuccessfulSearchAndAssertSearchAsync(
                request,
                expectedInResults: [seed[0], seed[1], seed[2]],
                notExpectedInResults: [seed[3]]);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.SuccessfulRequest);
    }

    [Fact]
    public async Task Search_ByLaEstab_Returns_No_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithLaEstab("1").Build(),
            SearchAggregateBuilder.Create().WithLaEstab("2").Build(),
            SearchAggregateBuilder.Create().WithLaEstab("10").Build(),
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "NOTHING")
                .Build();

        // Act
        UseCaseResponse<SearchResponse> response =
            await ExecuteSuccessfulSearchAndAssertSearchAsync(
                request,
                expectedInResults: [],
                notExpectedInResults: seed);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.SuccessfulRequest);
        Assert.Empty(response.Model.SearchProviderResults!.SearchResultCollection);
    }
}
