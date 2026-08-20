using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;

public sealed class SearchExactMatchBehaviourTests : SearchBehaviourTestsBase
{
    private const string SearchTermKey = "term-1";

    public SearchExactMatchBehaviourTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override Dictionary<string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>> ConfigureSearchTerm() => new()
    {
        {  SearchTermKey, [(builder) => builder.WithFieldName(DefaultSearchField).AppendExactMatchBehaviour()] }
    };

    [Fact]
    public async Task Returns_Exact_Match_Only_Case_Sensitive()
    {
        // arrange
        string searchTerm = "School";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "School")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "College")
                .Build(),
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "school")
                .Build(),
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "ScHoOl")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Does_Not_Return_Partial_Matches()
    {
        // arrange
        string searchTerm = "School";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "School")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "My School Academy")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "School Academy")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "Secondary School")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
