using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration.SearchConfiguration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Fields;

public sealed class SearchChainingFieldsWithAndTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";

    public SearchChainingFieldsWithAndTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {

    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            [
                (field) =>
                    field
                        .WithFieldName(DefaultSearchFieldName)
                        .AppendExactMatchBehaviour(),
                (field) =>
                    field
                        .WithFieldName(SecondarySearchFieldName)
                        .AppendContainsMatchBehaviour(),
            ],
            ChainFieldsBehaviour.AND);
    }

    [Fact]
    public async Task Returns_Intersection_Of_Matches_Of_All_Fields_When_And_Chained()
    {
        // arrange
        string searchTerm = "school";

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .SetValue(SecondarySearchFieldName, "My school")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .SetValue(SecondarySearchFieldName, "College")
                .Build(),

        SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "Academy")
                .SetValue(SecondarySearchFieldName, "My school")
                .Build(),

        SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "Academy")
                .SetValue(SecondarySearchFieldName, "College")
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

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

        SearchAggregate[] matchingEstablishments = [];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "Academy")
                .SetValue(SecondarySearchFieldName, "College")
                .Build(),

        SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "University")
                .SetValue(SecondarySearchFieldName, "Institute")
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }
}
