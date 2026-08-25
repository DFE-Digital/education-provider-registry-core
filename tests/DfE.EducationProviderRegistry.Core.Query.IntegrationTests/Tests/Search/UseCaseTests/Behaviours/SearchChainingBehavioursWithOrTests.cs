using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests.Behaviours;

public sealed class SearchChainingBehavioursWithOrTests : SearchUseCaseBase
{
    private const string SearchTermKey = "term-1";

    public SearchChainingBehavioursWithOrTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override (string, string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
    [
        (
                SearchTermKey,
                IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
                [
                    (builder) =>
                        builder.WithFieldName(DefaultSearchFieldName)
                            .AppendExactMatchBehaviour()
                            .AppendPartialMatchBehaviour(behaviourChainingPredicate: IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE)
                ]
            )
    ];

    [Fact]
    public async Task Returns_Matches_From_All_Behaviours_When_Chained_With_Or()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "My school")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "College")
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
