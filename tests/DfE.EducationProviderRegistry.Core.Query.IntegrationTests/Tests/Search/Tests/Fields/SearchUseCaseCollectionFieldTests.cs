using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Fields;

public sealed class SearchUseCaseCollectionFieldTests : SearchUseCaseMatchesResultsTestBase
{
    private const string SearchTermKey = "term-1";
    private readonly string _collectionFieldName = string.Empty; // Does not exist in SearchAggregate

    public SearchUseCaseCollectionFieldTests(
        IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder.WithSearchTerm(
            SearchTermKey,
            (field) =>
                field.WithFieldName(_collectionFieldName)
                    .AppendContainsMatchBehaviour());
    }

    [Fact(Skip = "Collection properties do not exist on SearchAggregate to fulfil this test")]
    public async Task Returns_Matches_From_Collection_Field()
    {
        // arrange
        const string searchTerm = "school";

        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithLocalAuthorityName("School Authority")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithLocalAuthorityName("College Authority")
                .Build()
        ];

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact(Skip = "Collection properties do not exist on SearchAggregate to fulfil this test")]
    public async Task Returns_Match_When_Any_Collection_Element_Matches()
    {
        // arrange
        const string searchTerm = "school";

        SearchAggregate establishment =
            SearchAggregateBuilder.Create()
                .WithLocalAuthorityName("College Authority")
                .WithLocalAuthorityName("School Authority")
                .Build();

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(
            [establishment], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectedInResults: [establishment],
            notExpectedInResults: []);
    }

    [Fact(Skip = "Collection properties do not exist on SearchAggregate to fulfil this test")]
    public async Task Does_Not_Return_Match_When_No_Collection_Elements_Match()
    {
        // arrange
        const string searchTerm = "school";

        SearchAggregate[] matchingEstablishments = [];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .WithLocalAuthorityName("College Authority")
                .WithLocalAuthorityName("School Authority")
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(
            nonMatchingEstablishments, TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act / assert
        await ExecuteAndAssertSearchAsync(
            request,
            expectedInResults: matchingEstablishments,
            notExpectedInResults: nonMatchingEstablishments);
    }
}
