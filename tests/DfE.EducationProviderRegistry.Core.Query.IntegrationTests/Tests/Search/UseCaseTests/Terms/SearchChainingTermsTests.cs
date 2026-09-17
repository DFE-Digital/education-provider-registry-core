using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests.Terms;

public sealed class SearchChainingTermsTests : SearchUseCaseBase
{
    private const string Term1Key = "what";
    private const string Term2Key = "where";

    public SearchChainingTermsTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override (
        string termKey,
        string chainFieldsWithPredicate,
        IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
    [
        (
        Term1Key,
        IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
        [
            builder =>
                builder
                    .WithFieldName(DefaultSearchFieldName)
                    .AppendExactMatchBehaviour()
        ]
    ),
    (
        Term2Key,
        IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
        [
            builder =>
                builder
                    .WithFieldName(SecondarySearchFieldName)
                    .AppendExactMatchBehaviour()
        ]
    )
    ];

    [Fact]
    public async Task Returns_Intersection_Of_Multiple_Search_Terms()
    {
        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .SetValue(SecondarySearchFieldName, "SW1A")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .SetValue(SecondarySearchFieldName, "M1")
                .Build(),

        SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "academy")
                .SetValue(SecondarySearchFieldName, "SW1A")
                .Build()
        ];

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [
                    (Term1Key, "school"),
                    (Term2Key, "SW1A")
                ],
                filters: []);

        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_No_Results_When_Any_Search_Term_Does_Not_Match()
    {
        // arrange
        SearchAggregate[] matchingEstablishments = [];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .SetValue(SecondarySearchFieldName, "M1")
                .Build(),

        SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "academy")
                .SetValue(SecondarySearchFieldName, "SW1A")
                .Build()
        ];

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [
                    (Term1Key, "school"),
                    (Term2Key, "SW1A")
                ],
                filters: []);

        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }

}
