using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Tests.Terms;

public sealed class SearchChainingTermsTests : SearchUseCaseMatchesResultsTestBase
{
    private const string Term1Key = "what";
    private const string Term2Key = "where";

    public SearchChainingTermsTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureSearchUseCase(SearchUseCaseConfigurationBuilder builder)
    {
        builder
            .WithSearchTerm(
                Term1Key,
                [
                    (field) =>
                        field.WithFieldName(DefaultSearchFieldName)
                            .AppendExactMatchBehaviour()
                ])
            .WithSearchTerm(
                Term2Key,
                [
                    (field) =>
                        field.WithFieldName(SecondarySearchFieldName)
                            .AppendExactMatchBehaviour()
                ]);
    }

    [Fact]
    public async Task Returns_Intersection_Of_Multiple_Search_Terms()
    {
        // arrange
        SearchAggregate[] matchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .SetValue(SecondarySearchFieldName, "SW1A")
                .Build()
        ];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .SetValue(SecondarySearchFieldName, "M1")
                .Build(),

        SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "academy")
                .SetValue(SecondarySearchFieldName, "SW1A")
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerms([
                    (Term1Key, "school"),
                    (Term2Key, "SW1A")
                ])
                .Build();


        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }

    [Fact]
    public async Task Returns_No_Results_When_Any_Search_Term_Does_Not_Match()
    {
        // arrange
        SearchAggregate[] matchingEstablishments = [];

        SearchAggregate[] nonMatchingEstablishments =
        [
            SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "school")
                .SetValue(SecondarySearchFieldName, "M1")
                .Build(),

        SearchAggregateBuilder.Create()
                .SetValue(DefaultSearchFieldName, "academy")
                .SetValue(SecondarySearchFieldName, "SW1A")
                .Build()
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(
            [.. matchingEstablishments, .. nonMatchingEstablishments], TestContext.Current.CancellationToken);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerms([
                    (Term1Key, "school"),
                    (Term2Key, "SW1A")
                ])
                .Build();

        await ExecuteAndAssertSearchAsync(
            request,
            matchingEstablishments,
            nonMatchingEstablishments);
    }

}
