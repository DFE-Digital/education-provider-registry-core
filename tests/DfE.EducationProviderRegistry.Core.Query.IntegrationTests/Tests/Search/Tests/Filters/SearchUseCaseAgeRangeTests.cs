using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Filters;

public sealed class SearchUseCaseFilterAgeRangeTests : SearchUseCaseMatchesResultsTestBase
{
    private const string DefaultedSearchTerm = "term-1";

    public SearchUseCaseFilterAgeRangeTests(IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            DefaultedSearchTerm,
            (field) =>
                field.WithFieldName(nameof(SearchAggregate.ProviderName))
                    .AppendContainsMatchBehaviour());

        builder.WithFilter(
            filterKey: "age_range",
            targetFilter: "AgeRangeFilter");
    }

    [Fact]
    public async Task Returns_Filtered_Results_When_Minimum_Age_Requested()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(5)
                .WithMaximumAge(10)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(10)
                .WithMaximumAge(15)
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(11)
                .WithMaximumAge(16)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(5)
                .WithMaximumAge(9)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments],
            TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .WithFilterRequest("age_range", ["10"])
                .Build();

        // act / assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Filtered_Results_When_Maximum_Age_Requested()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(5)
                .WithMaximumAge(10)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(10)
                .WithMaximumAge(15)
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(11)
                .WithMaximumAge(16)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(5)
                .WithMaximumAge(9)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments],
            TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .WithFilterRequest("age_range", [null!, "10"])
                .Build();

        // act / assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Filtered_Results_When_Age_Range_Requested()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(5)
                .WithMaximumAge(10)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(7)
                .WithMaximumAge(10)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(5)
                .WithMaximumAge(15)
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(5)
                .WithMaximumAge(7)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(8)
                .WithMaximumAge(10)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(7)
                .WithMaximumAge(9)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(10)
                .WithMaximumAge(15)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments],
            TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .WithFilterRequest("age_range", ["7", "10"])
                .Build();

        // act / assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_No_Results_When_Age_Range_Does_Not_Match()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments = [];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(3)
                .WithMaximumAge(6)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(8)
                .WithMaximumAge(9)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(11)
                .WithMaximumAge(16)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            nonMatchingEstablishments,
            TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .WithFilterRequest("age_range", ["7", "10"])
                .Build();

        // act / assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_All_Search_Matches_When_No_Age_Range_Filter_Provided()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(3)
                .WithMaximumAge(11)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithMinimumAge(11)
                .WithMaximumAge(18)
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName("academy")
                .WithMinimumAge(3)
                .WithMaximumAge(18)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments],
            TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .Build();

        // act / assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }
}
