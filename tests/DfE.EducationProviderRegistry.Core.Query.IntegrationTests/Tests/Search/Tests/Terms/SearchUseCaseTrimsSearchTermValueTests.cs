using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Terms;

public sealed class SearchUseCaseTrimsSearchTermValueTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";

    public SearchUseCaseTrimsSearchTermValueTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder
            .WithSearchTerm(
                SearchTermKey,
                [
                    (field) =>
                        field.WithFieldName(DefaultSearchFieldName)
                            .AppendContainsMatchBehaviour()
                ]);
    }

    [Fact]
    public async Task Ignores_Leading_And_Trailing_Whitespace_In_Search_Terms()
    {
        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
            .SetValue(DefaultSearchFieldName, "school")
            .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
            .SetValue(DefaultSearchFieldName, "College")
            .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            matchingEstablishments,
            TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "  sch ")
                .Build();

        // act / assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }
}
