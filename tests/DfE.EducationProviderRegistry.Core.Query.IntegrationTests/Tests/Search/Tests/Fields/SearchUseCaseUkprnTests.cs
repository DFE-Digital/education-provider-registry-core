using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Fields;

public sealed class SearchUseCaseUkprnTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    public SearchUseCaseUkprnTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            (field) =>
                field.WithFieldName(nameof(SearchAggregate.UkProviderReferenceNumber))
                    .AppendContainsMatchBehaviour()
                    .AppendStartsWithMatchBehaviour());
    }

    [Fact]
    public async Task Search_ByUkprn_Returns_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithUkprn("1").Build(),
            SearchAggregateBuilder.Create().WithUkprn("2").Build(),
            SearchAggregateBuilder.Create().WithUkprn("10").Build(),
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "1")
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
    public async Task Search_ByUkprn_No_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithUkprn("1").Build(),
            SearchAggregateBuilder.Create().WithUkprn("2").Build(),
            SearchAggregateBuilder.Create().WithUkprn("10").Build(),
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
