using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;

public sealed class SearchChainingBehavioursWithOrTests : SearchBehaviourTestsBase
{
    private const string OR_CHAINING_PREDICATE = "OR";

    public SearchChainingBehavioursWithOrTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureIndexedField(IndexedFieldConfigurationBuilder builder)
        => builder
            .AppendExactMatchBehaviour(behaviourChainingPredicate: OR_CHAINING_PREDICATE)
            .AppendPartialMatchBehaviour(behaviourChainingPredicate: OR_CHAINING_PREDICATE);


    [Fact]
    public async Task Returns_Matches_From_All_Behaviours_When_Chained_With_Or()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "school")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "My school")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "College")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            searchTerm,
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
