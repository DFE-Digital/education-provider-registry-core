using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;

public sealed class SearchPartialMatchBehaviourTests : SearchBehaviourTestsBase
{
    private const string SearchTermKey = "term-1";
    public SearchPartialMatchBehaviourTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override Dictionary<string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>> ConfigureSearchTerm() => new()
    {
        {  SearchTermKey, [(builder) => builder.WithFieldName(DefaultSearchField).AppendPartialMatchBehaviour()] }
    };

    [Fact]
    public async Task Returns_Matches_When_Search_Term_Is_A_Substring_Of_The_Value()
    {
        // arrange
        string searchTerm = "sch";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "School")
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

    [Fact]
    public async Task Returns_Matches_When_Search_Term_Appears_At_The_Start_Of_The_Value()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "School Academy")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "Academy College")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Matches_When_Search_Term_Appears_In_The_Middle_Of_The_Value()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "My School Academy")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "Academy College")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            [(SearchTermKey, searchTerm)],
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Matches_When_Search_Term_Appears_At_The_End_Of_The_Value()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "Secondary School")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "Academy College")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
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
                .SetValue(DefaultSearchField, "SCHOOL")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "School")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(DefaultSearchField, "school")
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
