using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;

public sealed class SearchChainingBehavioursWithAndTests : SearchBehaviourTestsBase
{
    public SearchChainingBehavioursWithAndTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureIndexedField(IndexedFieldConfigurationBuilder builder)
        => builder
            .AppendExactMatchBehaviour()
            .AppendPartialMatchBehaviour(behaviourChainingPredicate: IndexedFieldConfigurationBuilder.AND_CHAINING_PREDICATE);

    [Fact]
    public async Task Returns_Intersection_Of_Matches_Of_All_Behaviours_When_And_Chained()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "school")
                .Build(),
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "College")
                .Build(),
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "My school")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            searchTerm,
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
