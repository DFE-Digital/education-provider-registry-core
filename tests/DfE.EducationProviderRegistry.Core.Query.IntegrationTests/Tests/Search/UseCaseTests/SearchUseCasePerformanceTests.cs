using System.Diagnostics;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observer.Postgres;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.UseCaseTests;

public sealed class SearchUseCasePerformanceTests : SearchUseCaseBase
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
                builder =>
                    builder
                        .WithFieldName(DefaultSearchFieldName)
                        .AppendExactMatchBehaviour(),

                builder =>
                    builder
                        .WithFieldName(SecondarySearchFieldName)
                        .AppendPartialMatchBehaviour()
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

        Establishment[] establishments =
        [
            .. Enumerable.Range(1, totalMatches)
                .Select(_ =>
                    SearchEstablishmentBuilder.Create()
                        .SetValue(DefaultSearchFieldName, searchTerm)
                        .Build()),

            .. Enumerable.Range(1, totalEstablishments - 1)
                .Select(counter =>
                    SearchEstablishmentBuilder.Create()
                        .SetValue(
                            DefaultSearchFieldName,
                            $"ZZZ-{counter}")
                        .Build())
        ];

        await SeedSearchEstablishments.SeedAsync(establishments, ct);

        SearchRequest request =
            SearchRequestFactory.BuildSearchRequest(
                searchTerms: [(SearchTermKey, searchTerm)],
                filters: []);

        // act
        await QueryCollector.StartAsync(ct);

        Stopwatch stopwatch = Stopwatch.StartNew();

        UseCaseResponse<SearchResponse> response = await ExecuteUseCase<SearchRequest, SearchResponse>(request);

        stopwatch.Stop();

        PostgresQueries queries = await QueryCollector.GetObservationsAsync(ct);

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
