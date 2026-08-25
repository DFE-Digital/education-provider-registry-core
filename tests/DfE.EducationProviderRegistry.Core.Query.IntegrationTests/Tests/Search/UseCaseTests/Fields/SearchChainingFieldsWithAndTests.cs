using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests.Fields;

public sealed class SearchChainingFieldsWithAndTests : SearchUseCaseBase
{
    private const string SearchTermKey = "term-1";

    public SearchChainingFieldsWithAndTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {

    }

    protected override (string, string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
        [
            (
                SearchTermKey,
                IndexedFieldConfigurationBuilder.AND_CHAINING_PREDICATE,
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
    public async Task Returns_Intersection_Of_Matches_Of_All_Fields_When_And_Chained()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "school")
            .SetValue(SecondarySearchFieldName, "My school")
            .Build()
        ];

        Establishment[] nonMatchingEstablishments =
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
            .SetValue(DefaultSearchFieldName, "Academy")
            .SetValue(SecondarySearchFieldName, "College")
            .Build()
        ];

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [(SearchTermKey, searchTerm)],
                filters: []);

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_No_Results_When_No_Fields_Match()
    {
        // arrange
        string searchTerm = "school";

        Establishment[] matchingEstablishments = [];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "Academy")
            .SetValue(SecondarySearchFieldName, "College")
            .Build(),

        SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "University")
            .SetValue(SecondarySearchFieldName, "Institute")
            .Build()
        ];

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [(SearchTermKey, searchTerm)],
                filters: []);

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
