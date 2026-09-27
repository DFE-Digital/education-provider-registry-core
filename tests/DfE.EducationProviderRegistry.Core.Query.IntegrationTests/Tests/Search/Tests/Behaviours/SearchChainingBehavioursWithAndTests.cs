using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Behaviours;

public sealed class SearchChainingBehavioursWithAndTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";

    public SearchChainingBehavioursWithAndTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            (field) =>
                field.WithFieldName(DefaultSearchFieldName)
                    .AppendExactMatchBehaviour()
                    .AppendContainsMatchBehaviour(behaviourChainingPredicate: IndexedFieldConfigurationBuilder.AND_CHAINING_PREDICATE));
    }

    [Fact]
    public async Task Returns_Intersection_Of_Matches_Of_All_Behaviours_When_And_Chained()
    {
        // arrange
        string searchTerm = "school";

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .Build(),
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "College")
                .Build(),
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "My school")
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
