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

    protected override Dictionary<string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>> ConfigureSearchTerm() => new()
    {
        {  SearchTermKey, [
            (builder) =>
                builder.WithFieldName(DefaultSearchField)
                    .AppendExactMatchBehaviour()
                    .AppendPartialMatchBehaviour(behaviourChainingPredicate: IndexedFieldConfigurationBuilder.AND_CHAINING_PREDICATE)]
        }
    };

    [Fact]
    public async Task Returns_Intersection_Of_Matches_Of_All_Behaviours_When_And_Chained()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "school")
                .Build(),
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "College")
                .Build(),
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "My school")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
