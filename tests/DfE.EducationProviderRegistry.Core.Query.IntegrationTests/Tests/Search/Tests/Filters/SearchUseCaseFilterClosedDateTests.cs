using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Filters;

public sealed class SearchUseCaseFilterClosedDateTests : SearchUseCaseMatchesResultsTestBase
{
    private const string DefaultedSearchTerm = "term-1";

    public SearchUseCaseFilterClosedDateTests(IServiceProvider testServicesProvider)
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
            filterKey: "closed_date",
            targetFilter: "ClosedDateFilter");
    }

    [Fact]
    public async Task Returns_Filtered_Results_When_ClosedDate_From_Requested()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2020, 1, 1))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2021, 1, 1))
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2019, 12, 31))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(null)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .WithFilterRequest("closed_date", ["2020-01-01"])
                .Build();

        // act / assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Filtered_Results_When_ClosedDate_To_Requested()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2019, 1, 1))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2020, 1, 1))
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2020, 1, 2))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(null)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .WithFilterRequest("closed_date", [null!, "2020-01-01"])
                .Build();

        // act / assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Filtered_Results_When_ClosedDate_Range_Requested()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2020, 1, 1))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2020, 6, 1))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2020, 12, 31))
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2019, 12, 31))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2021, 1, 1))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(null)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .WithFilterRequest("closed_date", ["2020-01-01", "2020-12-31"])
                .Build();

        // act / assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_No_Results_When_ClosedDate_Does_Not_Match()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments = [];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2019, 1, 1))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2021, 1, 1))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(null)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            nonMatchingEstablishments, TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .WithFilterRequest("closed_date", ["2020-01-01", "2020-12-31"])
                .Build();

        // act / assert
        await ExecuteSuccessfulSearchAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_All_Search_Matches_When_No_ClosedDate_Filter_Provided()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2010, 1, 1))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(new DateOnly(2020, 1, 1))
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithClosedDate(null)
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName("academy")
                .WithClosedDate(new DateOnly(2020, 1, 1))
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableSearchAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

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
