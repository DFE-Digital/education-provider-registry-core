namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observor;

public sealed class PostgresQueryObservation
{
    public IReadOnlyList<PgStatStatementsDataTransferObject> Statements { get; init; } = [];
}
