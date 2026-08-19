using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;

public sealed class SearchFuzzyMatchBehaviourTests : SearchBehaviourTestsBase
{
    public SearchFuzzyMatchBehaviourTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureIndexedField(IndexedFieldConfigurationBuilder builder) => builder.WithFuzzyMatchBehaviour();

    [Fact]
    public async Task Returns_Similar_Words()
    {
        // arrange
        string searchTerm = "School";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "School")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "Schools")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "College")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "University")
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

    [Fact]
    public async Task Does_Not_Return_Completely_Unrelated_Words()
    {
        // arrange
        string searchTerm = "School";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "School")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "Banana")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "College")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "University")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            searchTerm,
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
