using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests.Fields;

public sealed class SearchUseCaseCollectionAndScalarFieldTests : SearchUseCaseBase
{
    private const string SearchTermKey = "term-1";

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
                        .AppendPartialMatchBehaviour()
            ]
        )
    ];

    [Fact]
    public async Task Returns_Intersection_Of_Scalar_And_Collection_Field_Matches()
    {
        // arrange
        const string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .WithAuthorityName("School Authority")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .WithAuthorityName("College Authority")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "academy")
                .WithAuthorityName("School Authority")
                .Build()
        ];

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [(SearchTermKey, searchTerm)],
                filters: []);

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
