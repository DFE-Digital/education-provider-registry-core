using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Fields;

public sealed class SearchUseCasePostcodeTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    public SearchUseCasePostcodeTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            (field) =>
                field.WithFieldName(nameof(SearchAggregate.Postcode))
                    .AppendExactMatchBehaviour()
                    .AppendContainsMatchBehaviour()
                    .AppendStartsWithMatchBehaviour());
    }

    [Fact]
    public async Task Search_ByPostcode_Returns_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithPostcode("B1").Build(),
            SearchAggregateBuilder.Create().WithPostcode("b1 a").Build(),
            SearchAggregateBuilder.Create().WithPostcode("EN1 B1X").Build(),
            SearchAggregateBuilder.Create().WithPostcode("AB2 3ER").Build(),
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "B1")
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
    public async Task Search_ByPostcode_No_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithPostcode("A1").Build(),
            SearchAggregateBuilder.Create().WithPostcode("SW15 1EX").Build(),
            SearchAggregateBuilder.Create().WithPostcode("BB BBB").Build(),
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "b1")
                .Build();

        // Act Assert
        UseCaseResponse<SearchResponse> response =
            await ExecuteSuccessfulSearchAndAssertSearchAsync(
                request,
                expectedInResults: [],
                notExpectedInResults: seed);

        Assert.NotNull(response);
        Assert.True(response.SuccessfulRequest);
        Assert.Empty(response.Model.SearchProviderResults!.SearchResultCollection);
    }
}
