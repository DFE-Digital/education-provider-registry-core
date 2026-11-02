using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Filters;

public sealed class SearchUseCaseFilterCollectionAndScalarFieldTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    private const string SenFacetKey = "sen";

    public SearchUseCaseFilterCollectionAndScalarFieldTests(
        IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(
        SearchUseCaseConfigurationBuilder builder)
    {
        builder
            .WithSearchTerm(
                SearchTermKey,
                field =>
                    field.WithFieldName(nameof(SearchAggregate.ProviderName))
                        .AppendExactMatchBehaviour());

        builder
            .WithFacet(SenFacetKey)
            .WithFacetDefinition(
                SenFacetKey,
                valueProperty: nameof(SearchAggregate.Sen))
            .WithFilter(
                SenFacetKey,
                concreteFilter: nameof(SenFilter));
    }

    [Fact]
    public async Task Returns_Intersection_Of_Scalar_Search_And_Collection_Filter_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchAggregate matchingEstablishment =
            SearchAggregateBuilder.Create()
                .WithProviderName("Test School")
                .WithSen("Other Provision", "SEN Support")
                .Build();

        SearchAggregate searchMatchOnly =
            SearchAggregateBuilder.Create()
                .WithProviderName("Test School")
                .WithSen("Other Provision")
                .Build();

        SearchAggregate filterMatchOnly =
            SearchAggregateBuilder.Create()
                .WithProviderName("Test Academy")
                .WithSen("SEN Support")
                .Build();

        await DatabaseFixture.SeedAsync<
            IEnumerable<SearchAggregate>,
            SearchableSearchAggregates>(
                [
                    matchingEstablishment,
                    searchMatchOnly,
                    filterMatchOnly
                ],
                ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "Test School")
                .WithFilterRequest(SenFacetKey, ["SEN Support"])
                .Build();

        // Act / Assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: [matchingEstablishment],
            notExpectedInResults:
            [
                searchMatchOnly,
                filterMatchOnly
            ]);
    }
}
