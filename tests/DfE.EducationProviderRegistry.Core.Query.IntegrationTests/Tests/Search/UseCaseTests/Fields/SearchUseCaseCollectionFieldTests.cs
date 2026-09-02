using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests.Fields;

public sealed class SearchUseCaseCollectionFieldTests
    : SearchUseCaseBase
{
    private const string SearchTermKey = "term-1";

    public SearchUseCaseCollectionFieldTests(
        IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
    }

    protected override (string termKey, string chainFieldsWithPredicate, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
    [
        (
            SearchTermKey,
            IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
            [
                builder =>
                    builder
                        .WithFieldName(CollectionFieldName)
                        .AppendContainsMatchBehaviour()
            ]
        )
    ];

    [Fact]
    public async Task Returns_Matches_From_Collection_Field()
    {
        // arrange
        const string searchTerm = "school";

        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .WithAuthorityName("School Authority")
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .WithAuthorityName("College Authority")
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
    public async Task Returns_Match_When_Any_Collection_Element_Matches()
    {
        // arrange
        const string searchTerm = "school";

        Establishment establishment =
            SearchEstablishmentBuilder.Create()
                .WithAuthorityName("College Authority")
                .WithAuthorityName("School Authority")
                .Build();

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [(SearchTermKey, searchTerm)],
                filters: []);

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectednResults: [establishment],
            notExpectedInResults: []);
    }

    [Fact]
    public async Task Does_Not_Return_Match_When_No_Collection_Elements_Match()
    {
        // arrange
        const string searchTerm = "school";

        Establishment[] matchingEstablishments = [];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .WithAuthorityName("College Authority")
                .WithAuthorityName("Academy Authority")
                .Build()
        ];

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [(SearchTermKey, searchTerm)],
                filters: []);

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectednResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }
}
