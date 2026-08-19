using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;

public sealed class SearchPartialMatchBehaviourTests : SearchBehaviourTestsBase
{
    public SearchPartialMatchBehaviourTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureIndexedField(IndexedFieldConfigurationBuilder builder) => builder.AppendPartialMatchBehaviour();

    [Fact]
    public async Task Returns_Matches_When_Search_Term_Is_A_Substring_Of_The_Value()
    {
        // arrange
        string searchTerm = "sch";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "School")
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

    [Fact]
    public async Task Returns_Matches_When_Search_Term_Appears_At_The_Start_Of_The_Value()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "School Academy")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "Academy College")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            searchTerm,
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
                .SetValue(SearchField, "My School Academy")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "Academy College")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            searchTerm,
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
                .SetValue(SearchField, "Secondary School")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "Academy College")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            searchTerm,
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
                .SetValue(SearchField, "SCHOOL")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "School")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "school")
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
