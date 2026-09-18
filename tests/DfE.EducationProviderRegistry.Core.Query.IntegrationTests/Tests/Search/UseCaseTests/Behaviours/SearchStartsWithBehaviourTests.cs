using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests.Behaviours;

public sealed class SearchStartsWithBehaviourTests : SearchMatchesUseCaseBaseTest
{
    private const string SearchTermKey = "term-1";

    public SearchStartsWithBehaviourTests(IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
    }

    protected override (string, string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
    [
        (
            SearchTermKey,
            IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
            [
                builder => builder
                    .WithFieldName(DefaultSearchFieldName)
                    .AppendStartsWithMatchBehaviour()
            ]
        )
    ];

    [Fact]
    public async Task Returns_Matches_When_Value_Starts_With_Search_Term()
    {
        // arrange
        string searchTerm = "school";

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "School")
                .Build(),

            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "School Academy")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "My School")
                .Build(),

            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "Secondary School")
                .Build(),

            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "College")
                .Build()
        ];

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Does_Not_Return_Matches_When_Search_Term_Appears_In_The_Middle_Of_The_Value()
    {
        // arrange
        string searchTerm = "school";

        SearchAggregate[] matchingEstablishments = [];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "My School Academy")
                .Build()
        ];

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Does_Not_Return_Matches_When_Search_Term_Appears_At_The_End_Of_The_Value()
    {
        // arrange
        string searchTerm = "school";

        SearchAggregate[] matchingEstablishments = [];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "Secondary School")
                .Build()
        ];

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Matches_Regardless_Of_Casing()
    {
        // arrange
        string searchTerm = "sChOoL";

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "SCHOOL")
                .Build(),

            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "School Academy")
                .Build(),

            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school college")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "My School")
                .Build(),

            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "College")
                .Build()
        ];

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Matches_When_Search_Term_Is_An_Exact_Match()
    {
        // arrange
        string searchTerm = "school";

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "School")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "My School")
                .Build(),

            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "College")
                .Build()
        ];

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
