using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;

public sealed class SearchChainingFieldsWithOrTests : SearchBehaviourTestsBase
{
    private const string SearchTermKey = "term-1";

    public SearchChainingFieldsWithOrTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
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
                            .AppendExactMatchBehaviour(),
                    (builder) =>
                        builder
                            .WithFieldName(SecondarySearchFieldName)
                            .AppendPartialMatchBehaviour(),
                ]
            )
    ];

    [Fact]
    public async Task Returns_Matches_From_First_Field_When_Or_Chained()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "school")
            .SetValue(SecondarySearchFieldName, "College")
            .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "Academy")
            .SetValue(SecondarySearchFieldName, "College")
            .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Matches_From_Second_Field_When_Or_Chained()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "Academy")
            .SetValue(SecondarySearchFieldName, "My school")
            .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "Academy")
            .SetValue(SecondarySearchFieldName, "College")
            .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Matches_From_Both_Fields_When_Or_Chained()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "school")
            .SetValue(SecondarySearchFieldName, "College")
            .Build(),

        SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "Academy")
            .SetValue(SecondarySearchFieldName, "My school")
            .Build(),

        SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "school")
            .SetValue(SecondarySearchFieldName, "My school")
            .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "Academy")
            .SetValue(SecondarySearchFieldName, "College")
            .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
