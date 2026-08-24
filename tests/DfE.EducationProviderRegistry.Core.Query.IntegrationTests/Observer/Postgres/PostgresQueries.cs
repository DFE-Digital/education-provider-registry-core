namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observer.Postgres;

public sealed class PostgresQueries
{
    public IReadOnlyList<PostgresQuery> Queries { get; init; } = [];

    public int Count => Queries.Count;

    public double TotalQueryExecutionTime => Queries.Sum(t => t.total_exec_time);
}
