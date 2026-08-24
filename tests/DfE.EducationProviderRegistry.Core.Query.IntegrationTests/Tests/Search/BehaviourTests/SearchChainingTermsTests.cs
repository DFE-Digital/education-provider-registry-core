using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.BehaviourTests;

public sealed class SearchChainingTermsTests : SearchBehaviourTestsBase
{
    private const string Term1Key = "what";
    private const string Term2Key = "where";

    public SearchChainingTermsTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override (
        string termKey,
        string chainFieldsWithPredicate,
        IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
    [
        (
        Term1Key,
        IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
        [
            builder =>
                builder
                    .WithFieldName(DefaultSearchFieldName)
                    .AppendExactMatchBehaviour()
        ]
    ),
    (
        Term2Key,
        IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
        [
            builder =>
                builder
                    .WithFieldName(SecondarySearchFieldName)
                    .AppendExactMatchBehaviour()
        ]
    )
    ];

    [Fact]
    public async Task Returns_Intersection_Of_Multiple_Search_Terms()
    {
        // arrange
        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "school")
            .SetValue(SecondarySearchFieldName, "SW1A")
            .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "school")
            .SetValue(SecondarySearchFieldName, "M1")
            .Build(),

        SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "academy")
            .SetValue(SecondarySearchFieldName, "SW1A")
            .Build()
        ];

        await ExecuteAndAssertSearchAsync(
            [
                (Term1Key, "school"),
                (Term2Key, "SW1A")
            ],
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_No_Results_When_Any_Search_Term_Does_Not_Match()
    {
        // arrange
        Establishment[] matchingEstablishments = [];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "school")
            .SetValue(SecondarySearchFieldName, "M1")
            .Build(),

        SearchEstablishmentBuilder.Create()
            .SetValue(DefaultSearchFieldName, "academy")
            .SetValue(SecondarySearchFieldName, "SW1A")
            .Build()
        ];

        await ExecuteAndAssertSearchAsync(
            [
                (Term1Key, "school"),
                (Term2Key, "SW1A")
            ],
            matchingEstablishments,
            nonMatchingEstablishments);
    }

}
