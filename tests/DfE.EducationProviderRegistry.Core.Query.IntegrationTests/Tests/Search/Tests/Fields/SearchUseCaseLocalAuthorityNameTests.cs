using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Fields;

public sealed class SearchUseCaseLocalAuthorityNameTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    public SearchUseCaseLocalAuthorityNameTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            (field) =>
                field.WithFieldName(nameof(SearchAggregate.LocalAuthorityName))
                    .AppendExactMatchBehaviour()
                    .AppendContainsMatchBehaviour()
                    .AppendStartsWithMatchBehaviour());
    }

    [Fact]
    public async Task Search_ByLocalAuthorityName_Returns_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithLocalAuthorityName("br").Build(),
            SearchAggregateBuilder.Create().WithLocalAuthorityName("Bristol").Build(),
            SearchAggregateBuilder.Create().WithLocalAuthorityName("bradford").Build(),
            SearchAggregateBuilder.Create().WithLocalAuthorityName("Birmingham").Build(),
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "br")
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
    public async Task Search_ByLocalAuthorityName_Returns_No_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithLaEstab("Bristol").Build(),
            SearchAggregateBuilder.Create().WithLaEstab("London").Build(),
            SearchAggregateBuilder.Create().WithLaEstab("Cardiff").Build(),
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
        Assert.True(response.SuccessfulRequest);
        Assert.Empty(response.Model.SearchProviderResults!.SearchResultCollection);
    }
}
