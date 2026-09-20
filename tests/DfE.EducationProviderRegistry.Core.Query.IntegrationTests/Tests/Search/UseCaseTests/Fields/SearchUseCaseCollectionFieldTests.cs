using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests.Fields;

public sealed class SearchUseCaseCollectionFieldTests
    : SearchMatchesUseCaseBaseTest
{
    private const string SearchTermKey = "term-1";
    private readonly string CollectionFieldName = string.Empty; // Does not exist in SearchAggregate

    public SearchUseCaseCollectionFieldTests(
        IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
    }

    protected override (string termKey, string chainFieldsWithPredicate, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
    [
        (
            SearchTermKey,
            IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
            [
                builder =>
                    builder
                        .WithFieldName(CollectionFieldName)
                        .AppendContainsMatchBehaviour()
            ]
        )
    ];

    [Fact(Skip = "Collection properties do not exist on SearchAggregate to fulfil this test")]
    public async Task Returns_Matches_From_Collection_Field()
    {
        // arrange
        const string searchTerm = "school";

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithLocalAuthorityName("School Authority")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithLocalAuthorityName("College Authority")
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

    [Fact(Skip = "Collection properties do not exist on SearchAggregate to fulfil this test")]
    public async Task Returns_Match_When_Any_Collection_Element_Matches()
    {
        // arrange
        const string searchTerm = "school";

        SearchAggregate establishment =
            SearchAggregateBuilder.Create()
                .WithLocalAuthorityName("College Authority")
                .WithLocalAuthorityName("School Authority")
                .Build();

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectednResults: [establishment],
            notExpectedInResults: []);
    }

    [Fact(Skip = "Collection properties do not exist on SearchAggregate to fulfil this test")]
    public async Task Does_Not_Return_Match_When_No_Collection_Elements_Match()
    {
        // arrange
        const string searchTerm = "school";

        SearchAggregate[] matchingEstablishments = [];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithLocalAuthorityName("College Authority")
                .WithLocalAuthorityName("School Authority")
                .Build()
        ];

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectednResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }
}
