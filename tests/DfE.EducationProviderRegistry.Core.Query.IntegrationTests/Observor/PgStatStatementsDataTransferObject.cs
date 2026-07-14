namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observor;

public sealed class PgStatStatementsDataTransferObject
{
    public string? query { get; set; }
    public int calls { get; set; }
    public double total_exec_time { get; set; }
}
