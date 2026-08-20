using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;

public sealed class SearchChainingBehavioursWithOrTests : SearchBehaviourTestsBase
{
    private const string SearchTermKey = "term-1";

    public SearchChainingBehavioursWithOrTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override Dictionary<string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>> ConfigureSearchTerm() => new()
    {
        {  SearchTermKey, [
            (builder) =>
                builder.WithFieldName(DefaultSearchField)
                    .AppendExactMatchBehaviour()
                    .AppendPartialMatchBehaviour(behaviourChainingPredicate: IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE)]
        }
    };

    [Fact]
    public async Task Returns_Matches_From_All_Behaviours_When_Chained_With_Or()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "school")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "My school")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "College")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
