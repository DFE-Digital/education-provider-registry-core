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

    protected override (string, string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
    [
        (
                SearchTermKey,
                IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
                [
                    (builder) => builder.WithFieldName(DefaultSearchFieldName).AppendExactMatchBehaviour()
                ]
            )
    ];

    [Fact]
    public async Task Returns_Exact_Match_Only_Case_Sensitive()
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
                .SetValue(DefaultSearchFieldName, "College")
                .Build(),
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .Build(),
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "ScHoOl")
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
                .SetValue(DefaultSearchFieldName, "School")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "My School Academy")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "School Academy")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchFieldName, "Secondary School")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
