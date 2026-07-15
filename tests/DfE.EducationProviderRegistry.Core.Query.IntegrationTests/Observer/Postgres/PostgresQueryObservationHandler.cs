using Dapper;
using Npgsql;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observor.Postgres;

// Maybe we can use a Clock injected to query between when GetObservationsAsync is called from StartAsync ->?
public sealed class PostgresQueryObservationHandler
    : IObservationHandler<PostgresQueryObservations>
{
    private readonly string _connectionString;

    public PostgresQueryObservationHandler(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
        await using NpgsqlConnection conn = new(_connectionString);
        await conn.ExecuteAsync("CREATE EXTENSION IF NOT EXISTS pg_stat_statements;");
        await conn.ExecuteAsync("SELECT pg_stat_statements_reset();");
    }

    public Task StopAsync() => Task.CompletedTask;

    public async Task<PostgresQueryObservations> GetObservationsAsync(CancellationToken ct = default)
    {
        const string sql = """
            SELECT query, calls, total_exec_time, mean_exec_time, rows 
            FROM pg_stat_statements
            WHERE query NOT ILIKE '%pg_stat_statements%'
            AND query <> 'DISCARD ALL'
        """;

        await using NpgsqlConnection conn = new(_connectionString);

        List<ObservedQuery> statements =
            (await conn.QueryAsync<ObservedQuery>(sql))
                .ToList();

        return new PostgresQueryObservations
        {
            Queries = statements
        };
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
