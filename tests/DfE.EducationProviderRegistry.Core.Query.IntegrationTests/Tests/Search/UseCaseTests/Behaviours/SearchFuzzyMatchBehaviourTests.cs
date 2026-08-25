using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests.Behaviours;

public sealed class SearchFuzzyMatchBehaviourTests : SearchBehaviourTestsBase
{
    private const string SearchTermKey = "term-1";
    public SearchFuzzyMatchBehaviourTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override (string, string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
    [
        (
            SearchTermKey,
            IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
            [
                (builder) =>
                    builder.WithFieldName(DefaultSearchFieldName).AppendFuzzyMatchBehaviour()
            ]
        )
    ];

    [Fact]
    public async Task Returns_Similar_Words()
    {
        // arrange
        string searchTerm = "School";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "School")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "Schools")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "College")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "University")
                .Build()
        ];

        // act / assert
        await ExecuteAndAssertSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Matches_Regardless_Of_Casing()
    {
        // arrange
        string searchTerm = "sChOoL";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "School")
                .Build(),
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "College")
                .Build()
        ];

        // act / assert
        await ExecuteAndAssertSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Does_Not_Return_Completely_Unrelated_Words()
    {
        // arrange
        string searchTerm = "School";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "School")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "Banana")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "College")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "University")
                .Build()
        ];

        // act / assert
        await ExecuteAndAssertSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
