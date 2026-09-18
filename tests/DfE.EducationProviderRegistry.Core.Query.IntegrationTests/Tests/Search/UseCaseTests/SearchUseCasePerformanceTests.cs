using System.Diagnostics;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Observer.Postgres;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests;

public sealed class SearchUseCasePerformanceTests : SearchMatchesUseCaseBaseTest
{
    private const string SearchTermKey = "term-1";

    public SearchUseCasePerformanceTests(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override (string, string, IEnumerable<Action<IndexedFieldConfigurationBuilder>>)[] CreateSearchTermsConfiguration() =>
    [
        (
            SearchTermKey,
            IndexedFieldConfigurationBuilder.OR_CHAINING_PREDICATE,
            [
                (builder) =>
                    builder
                        .WithFieldName(DefaultSearchFieldName)
                        .AppendExactMatchBehaviour(),

                (builder) =>
                    builder
                        .WithFieldName(SecondarySearchFieldName)
                        .AppendContainsMatchBehaviour()
            ]
        )
    ];

    [Theory]
    [InlineData(1_000, 1)]
    [InlineData(10_000, 100)]
    [InlineData(50_000, 1_000)]
    public async Task Captures_Query_Performance(int totalEstablishments, int totalMatches)
    {
        // arrange
        CancellationToken ct =
            TestContext.Current.CancellationToken;

        const string searchTerm = "school";

        IReadOnlyCollection<SearchAggregate> establishments =
        [
            .. Enumerable.Range(1, totalMatches)
                .Select(_ =>
                    SearchAggregateBuilder.Create()
                        .SetValue(DefaultSearchFieldName, searchTerm)
                        .Build()),

            .. Enumerable.Range(1, totalEstablishments - 1)
                .Select(counter =>
                    SearchAggregateBuilder.Create()
                        .SetValue(
                            DefaultSearchFieldName,
                            $"ZZZ-{counter}")
                        .Build())
        ];

        await DatabaseFixture.SeedAsync<IEnumerable<SearchAggregate>, SearchableAggregates>(establishments, ct);

        SearchRequest request =
            SearchRequestBuilder.Create()
                .WithSearchTerm(SearchTermKey, searchTerm)
                .Build();

        // act
        await DatabaseFixture.QueryCollector!.StartAsync(ct);

        Stopwatch stopwatch = Stopwatch.StartNew();

        UseCaseResponse<SearchResponse> response = await ExecuteUseCase<SearchRequest, SearchResponse>(request, ct);

        stopwatch.Stop();

        PostgresQueries queries = await DatabaseFixture.QueryCollector.GetObservationsAsync(ct);

        // assert
        Assert.NotNull(response);

        // Assumption: total operation should take no longer than 2s
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(2));

        // 1 query for retrieving matches, 1 query for Facets
        Assert.Equal(3, queries.Count);

        // Assumption: Search queries should execute within 1s
        TimeSpan expectedQueryExecutionTime = TimeSpan.FromSeconds(1);

        Assert.True(
            queries.TotalQueryExecutionTime < expectedQueryExecutionTime.TotalMilliseconds,
                $"search queries total execution time {queries.TotalQueryExecutionTime}ms exceeded){expectedQueryExecutionTime.TotalMilliseconds}ms");
    }
}
