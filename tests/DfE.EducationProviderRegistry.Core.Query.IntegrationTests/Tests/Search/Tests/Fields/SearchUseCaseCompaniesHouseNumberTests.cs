using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Fields;

public sealed class SearchUseCaseCompaniesHouseNumberTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    public SearchUseCaseCompaniesHouseNumberTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            (field) =>
                field.WithFieldName(nameof(SearchAggregate.CompaniesHouseNumber))
                    .AppendContainsMatchBehaviour()
                    .AppendStartsWithMatchBehaviour()
                    .AppendExactMatchBehaviour());
    }

    [Fact]
    public async Task Search_ByCompaniesHouseNumber_Returns_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithCompaniesHouseNumber("val").Build(),
            SearchAggregateBuilder.Create().WithCompaniesHouseNumber("value-1").Build(),
            SearchAggregateBuilder.Create().WithCompaniesHouseNumber("VALUE-2").Build(),
            SearchAggregateBuilder.Create().WithCompaniesHouseNumber("number-3").Build(),
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(seed, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "val")
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
    public async Task Search_ByCompaniesHouseNumber_No_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        IReadOnlyList<SearchAggregate> seed = [
            SearchAggregateBuilder.Create().WithCompaniesHouseNumber("STUB-1").Build(),
            SearchAggregateBuilder.Create().WithCompaniesHouseNumber("STUB-2").Build(),
            SearchAggregateBuilder.Create().WithCompaniesHouseNumber("STUB-3").Build(),
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
