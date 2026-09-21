using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Filters;

public sealed class SearchUseCaseFiltersTests : SearchUseCaseMatchesResultsTestBase
{
    private const string DefaultedSearchTerm = "term-1";
    public SearchUseCaseFiltersTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            DefaultedSearchTerm,
            (field) =>
                field.WithFieldName(nameof(SearchAggregate.ProviderName))
                    .AppendContainsMatchBehaviour());

        builder.WithFilter(filterKey: "searchprovidertypeid", concreteFilter: "SearchProviderTypeFilter");
    }

    [Fact]
    public async Task Returns_Filtered_Results_When_FilterValue_Requested()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithProviderTypeId(1)
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithProviderTypeId(2)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .WithFilterRequest(new FilterRequest("searchprovidertypeid", [1]))
                .Build();

        // act // assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Results_Matching_Any_Filter_Value()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithProviderTypeId(1)
                .Build(),

            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithProviderTypeId(2)
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithProviderTypeId(3)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .WithFilterRequest(new FilterRequest("searchprovidertypeid", [1, 2]))
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_No_Results_When_Filter_Does_Not_Match()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments = [];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithProviderTypeId(1)
                .Build(),

        SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithProviderTypeId(2)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .WithFilterRequest(new FilterRequest("searchprovidertypeid", [999]))
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_All_Search_Matches_When_No_Filters_Provided()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithProviderTypeId(1)
                .Build(),

        SearchAggregateBuilder.Create()
                .WithProviderName(stubEstablishmentMatchesName)
                .WithProviderTypeId(2)
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithProviderName("academy")
                .WithProviderTypeId(1)
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(DefaultedSearchTerm, stubEstablishmentMatchesName)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }
}
