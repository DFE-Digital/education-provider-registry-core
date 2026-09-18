using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests.Behaviours;

public sealed class SearchExactMatchBehaviourTests : SearchMatchesUseCaseBaseTest
{
    private const string SearchTermKey = "term-1";

    public SearchExactMatchBehaviourTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override (string, string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
    [
        (
                SearchTermKey,
                IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
                [
                    (builder) => builder.WithFieldName(DefaultSearchFieldName).AppendExactMatchBehaviour()
                ]
            )
    ];

    [Fact]
    public async Task Returns_Exact_Match_Only_Case_Sensitive()
    {
        // arrange
        string searchTerm = "School";

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "School")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "College")
                .Build(),
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .Build(),
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "ScHoOl")
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
    public async Task Does_Not_Return_Partial_Matches()
    {
        // arrange
        string searchTerm = "School";

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "School")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "My School Academy")
                .Build(),

            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "School Academy")
                .Build(),

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
}
