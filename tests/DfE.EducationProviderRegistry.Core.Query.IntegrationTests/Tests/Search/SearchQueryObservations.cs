using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observer.Postgres;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search;

internal sealed class SearchQueryObservations
{
    private readonly IReadOnlyList<PostgresQuery> _queries;

    public SearchQueryObservations(PostgresQueries queries)
    {
        _queries =
           [
            .. queries.Queries
                .Where(query => query.query != "DISCARD ALL")
           ];
    }

    private PostgresQuery InitialProjection =>
        _queries.First(
            (t) => t.query?.Contains("LEFT JOIN core.site AS s") ?? false);

    private PostgresQuery TrigramSearch => GetSingleQuery("similarity(");
    private PostgresQuery Facets => GetSingleQuery("GROUP BY e.establishment_type_id");

    public void AssertSearchPipelineExecuted(TimeSpan expectedExecutionTime)
    {
        // Projection called once
        Assert.Same(InitialProjection, _queries[0]);
        Assert.Equal(1, InitialProjection.calls);

        // Trigram called once
        Assert.Same(TrigramSearch, _queries[1]);
        Assert.Equal(1, TrigramSearch.calls);

        // Facets called once
        Assert.Same(Facets, _queries[2]);
        Assert.Equal(1, Facets.calls);

        TimeSpan totalExecutionTime = TimeSpan.FromMilliseconds(InitialProjection.total_exec_time + TrigramSearch.total_exec_time + Facets.total_exec_time);

        Assert.True(
            totalExecutionTime < expectedExecutionTime,
            userMessage: $"search queries total execution time {totalExecutionTime.TotalMilliseconds}ms exceeded {expectedExecutionTime.TotalMilliseconds}ms");
    }

    private PostgresQuery GetSingleQuery(string contains) =>
        _queries.SingleOrDefault(
            t => t.query?.Contains(contains, StringComparison.Ordinal) ?? false)
                ?? throw new InvalidOperationException(
                    $"Could not find query containing '{contains}'.");
}
