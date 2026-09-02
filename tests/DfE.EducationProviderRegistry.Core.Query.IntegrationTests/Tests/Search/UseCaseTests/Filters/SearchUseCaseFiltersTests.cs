using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests.Filters;

public sealed class SearchUseCaseFiltersTests : SearchUseCaseBase
{
    private const string DefaultedSearchTerm = "term-1";
    public SearchUseCaseFiltersTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override (string termKey, string chainFieldsWithPredicate, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
        [
            (
                DefaultedSearchTerm,
                IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
                [
                    builder =>
                    builder
                        .WithFieldName(nameof(Establishment.Name))
                        .AppendContainsMatchBehaviour()
                ]
            )
        ];

    protected override IEnumerable<KeyValuePair<string, string?>> CreateFilterExpressionOptions()
    {
        string filterRequestKey = "EstablishmentTypeId";
        string concreteFilterInRegistry = "EstablishmentTypeFilter";

        return [
            new(filterRequestKey, concreteFilterInRegistry)
        ];
    }

    [Fact]
    public async Task Returns_Filtered_Results_When_FilterValue_Requested()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .WithName(stubEstablishmentMatchesName)
                .WithEstablishmentTypeId(1)
                .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
                .WithName(stubEstablishmentMatchesName)
                .WithEstablishmentTypeId(2)
                .Build()
        ];

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [(DefaultedSearchTerm, stubEstablishmentMatchesName)],
                filters: [
                    new FilterRequest("EstablishmentTypeId", [1])
                ]);

        // act // assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectednResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_Results_Matching_Any_Filter_Value()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .WithName(stubEstablishmentMatchesName)
            .WithEstablishmentTypeId(1)
            .Build(),

        SearchEstablishmentBuilder.Create()
            .WithName(stubEstablishmentMatchesName)
            .WithEstablishmentTypeId(2)
            .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .WithName(stubEstablishmentMatchesName)
            .WithEstablishmentTypeId(3)
            .Build()
        ];

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [(DefaultedSearchTerm, stubEstablishmentMatchesName)],
                filters: [
                    new FilterRequest("EstablishmentTypeId", [1, 2])
                ]);

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectednResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_No_Results_When_Filter_Does_Not_Match()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        Establishment[] matchingEstablishments = [];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .WithName(stubEstablishmentMatchesName)
            .WithEstablishmentTypeId(1)
            .Build(),

        SearchEstablishmentBuilder.Create()
            .WithName(stubEstablishmentMatchesName)
            .WithEstablishmentTypeId(2)
            .Build()
        ];

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [(DefaultedSearchTerm, stubEstablishmentMatchesName)],
                filters: [
                    new FilterRequest("EstablishmentTypeId", [999])
                    ]);

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectednResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_All_Search_Matches_When_No_Filters_Provided()
    {
        const string stubEstablishmentMatchesName = "school";

        // arrange
        Establishment[] matchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .WithName(stubEstablishmentMatchesName)
            .WithEstablishmentTypeId(1)
            .Build(),

        SearchEstablishmentBuilder.Create()
            .WithName(stubEstablishmentMatchesName)
            .WithEstablishmentTypeId(2)
            .Build()
        ];

        Establishment[] nonMatchingEstablishments =
        [
            SearchEstablishmentBuilder.Create()
            .WithName("academy")
            .WithEstablishmentTypeId(1)
            .Build()
        ];

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [(DefaultedSearchTerm, stubEstablishmentMatchesName)],
                filters: []);

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectednResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }

    // TODO as more filters are added
    // Returns_Intersection_Of_Multiple_Filters()
    /*
     * 
     * [Fact]
public async Task Returns_Intersection_Of_Multiple_Filters()
{
    const string stubEstablishmentMatchesName = "school";

    // arrange
    Establishment[] matchingEstablishments =
    [
        SearchEstablishmentBuilder.Create()
            .WithName(stubEstablishmentMatchesName)
            .WithEstablishmentTypeId(1)
            .WithEstablishmentStatusId(1)
            .Build()
    ];

    Establishment[] nonMatchingEstablishments =
    [
        SearchEstablishmentBuilder.Create()
            .WithName(stubEstablishmentMatchesName)
            .WithEstablishmentTypeId(1)
            .WithEstablishmentStatusId(2)
            .Build(),

        SearchEstablishmentBuilder.Create()
            .WithName(stubEstablishmentMatchesName)
            .WithEstablishmentTypeId(2)
            .WithEstablishmentStatusId(1)
            .Build(),

        SearchEstablishmentBuilder.Create()
            .WithName(stubEstablishmentMatchesName)
            .WithEstablishmentTypeId(2)
            .WithEstablishmentStatusId(2)
            .Build()
    ];

    List<FilterRequest> filters =
    [
        new FilterRequest("EstablishmentTypeId", [1]),
        new FilterRequest("EstablishmentStatusId", [1])
    ];

    // act / assert
    await ExecuteAndAssertSearchAsync(
        searchTerms:
        [
            (DefaultedSearchTerm, stubEstablishmentMatchesName)
        ],
        matchSearchTerm: matchingEstablishments,
        nonMatchSearchTerm: nonMatchingEstablishments,
        filters);
}
     * 
     */
}
