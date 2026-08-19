using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;

public sealed class SearchExactMatchBehaviourTests : SearchBehaviourTestsBase
{
    public SearchExactMatchBehaviourTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureIndexedField(IndexedFieldConfigurationBuilder builder) => builder.AppendExactMatchBehaviour();

    [Fact]
    public async Task Returns_Exact_Match_Only_Case_Sensitive()
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
                .SetValue(SearchField, "College")
                .Build(),
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "school")
                .Build(),
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "ScHoOl")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            searchTerm,
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
                .SetValue(SearchField, "School")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "My School Academy")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "School Academy")
                .Build(),

            SearchEstablishmentBuilder.Create()
                .SetValue(SearchField, "Secondary School")
                .Build()
        ];

        // act / assert
        await AssertExecutedSearchAsync(
            searchTerm,
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
