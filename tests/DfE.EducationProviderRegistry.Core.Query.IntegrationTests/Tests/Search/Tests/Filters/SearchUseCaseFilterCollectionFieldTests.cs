using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering.Filters;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Filters;

public sealed class SearchUseCaseFilterCollectionFieldTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    private const string SenFacetKey = "sen";

    public SearchUseCaseFilterCollectionFieldTests(
        IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(
        SearchUseCaseConfigurationBuilder builder)
    {
        // Stub a Search
        builder
            .WithSearchTerm(SearchTermKey,
                field =>
                    field.WithFieldName(nameof(SearchAggregate.ProviderName))
                        .AppendContainsMatchBehaviour());
        builder
            .WithFacet(SenFacetKey)
            .WithFacetDefinition(SenFacetKey, valueProperty: nameof(SearchAggregate.Sen))
            .WithFilter(SenFacetKey, targetFilter: nameof(SenFilter));

    }

    [Fact]
    public async Task Returns_Matches_From_Collection_Field()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName("Test School")
                .WithSen("SEN Support")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName("Test School")
                .WithSen("Other Provision")
                .Build()
        ];

        await DatabaseFixture.SeedAsync<
            IEnumerable<SearchAggregate>,
            SearchableSearchAggregates>(
                [.. matchingEstablishments, .. nonMatchingEstablishments],
                ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "Test School")
                .WithFilterRequest(SenFacetKey, ["SEN Support"])
                .Build();

        // Act / Assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Match_When_Any_Collection_Element_Matches()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchAggregate matchingEstablishment =
            SearchAggregateBuilder.Create()
                .WithProviderName("Test School")
                .WithSen("Other Provision", "SEN Support", "Another Provision")
                .Build();

        SearchAggregate nonMatchingEstablishment =
            SearchAggregateBuilder.Create()
                .WithProviderName("Test School")
                .WithSen("Other Provision", "Another Provision")
                .Build();

        await DatabaseFixture.SeedAsync<
            IEnumerable<SearchAggregate>,
            SearchableSearchAggregates>(
                [
                    matchingEstablishment,
                    nonMatchingEstablishment
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
            notExpectedInResults: [nonMatchingEstablishment]);
    }

    [Fact]
    public async Task Does_Not_Return_Match_When_No_Collection_Elements_Match()
    {
        // Arrange
        CancellationToken ct = TestContext.Current.CancellationToken;

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName("Test School")
                .WithSen("Other Provision", "Another Provision")
                .Build()
        ];

        await DatabaseFixture.SeedAsync<
            IEnumerable<SearchAggregate>,
            SearchableSearchAggregates>(
                nonMatchingEstablishments,
                ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, "Test School")
                .WithFilterRequest(SenFacetKey, ["SEN Support"])
                .Build();

        // Act / Assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: [],
            notExpectedInResults: nonMatchingEstablishments);
    }
}
