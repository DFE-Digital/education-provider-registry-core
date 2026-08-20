using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;

public sealed class SearchChainingBehavioursWithAndTests : SearchBehaviourTestsBase
{
    private const string SearchTermKey = "term-1";

    public SearchChainingBehavioursWithAndTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override (string, string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
    [
        (
            SearchTermKey,
            IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
            [
                (builder) =>
                    builder
                        .WithFieldName(DefaultSearchFieldName)
                        .AppendExactMatchBehaviour()
                        .AppendPartialMatchBehaviour(behaviourChainingPredicate: IndexedFieldConfigurationBuilder.AND_CHAINING_PREDICATE)
            ]
        )
    ];

    [Fact]
    public async Task Returns_Intersection_Of_Matches_Of_All_Behaviours_When_And_Chained()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .Build(),
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "College")
                .Build(),
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "My school")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
