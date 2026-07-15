namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observor.Postgres;

public sealed class PostgresQueryObservations
{
    public IReadOnlyList<ObservedQuery> Queries { get; init; } = [];

    public int Count => Queries.Count;
}
