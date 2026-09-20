using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests.Fields;

public sealed class SearchUseCaseCollectionAndScalarFieldTests : SearchMatchesUseCaseBaseTest
{
    private const string SearchTermKey = "term-1";
    private readonly string CollectionFieldName = string.Empty; // does not exist

    public SearchUseCaseCollectionAndScalarFieldTests(
        IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
    }

    protected override (
        string termKey,
        string chainFieldsWithPredicate,
        IEnumerable<Action<IndexedFieldConfigurationBuilder>>
    )[] CreateSearchTermsConfiguration() =>
    [
        (
            SearchTermKey,
            IndexedFieldConfigurationBuilder.AND_CHAINING_PREDICATE,
            [
                builder =>
                    builder
                        .WithFieldName(DefaultSearchFieldName)
                        .AppendExactMatchBehaviour(),

                builder =>
                    builder
                        .WithFieldName(CollectionFieldName)
                        .AppendContainsMatchBehaviour()
            ]
        )
    ];

    [Fact(Skip = "Collection properties do not exist on SearchAggregate to fulfil this test")]
    public async Task Returns_Intersection_Of_Scalar_And_Collection_Field_Matches()
    {
        // arrange
        const string searchTerm = "school";

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .WithLocalAuthorityName("School Authority")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .WithLocalAuthorityName("College Authority")
                .Build(),

            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "academy")
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
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
