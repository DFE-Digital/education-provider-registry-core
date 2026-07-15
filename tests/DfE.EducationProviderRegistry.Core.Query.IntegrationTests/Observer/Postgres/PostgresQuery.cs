namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observer.Postgres;

// pg_stat table
public sealed record PostgresQuery
{
    public string? query { get; set; }
    public int calls { get; set; }
    public double total_exec_time { get; set; } // ms
    public double mean_exec_time { get; set; } // ms
    public long rows { get; set; }
}
