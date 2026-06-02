using IntegrationTests.Database.Server.Abstractions;
using IntegrationTests.Database.Server.Postgres.Container;

namespace IntegrationTests.Database.Server.Postgres;

internal sealed class PostgresDatabaseFactory : IDatabaseFactory
{
    private readonly PostgresContainerOptions _containerOptions;
    private readonly PostgresDatabaseOptions _databaseOptions;
    private readonly IContainerFactory _containerFactory;

    public PostgresDatabaseFactory(
        PostgresContainerOptions containerOptions,
        PostgresDatabaseOptions databaseOptions,
        IContainerFactory containerFactory)
    {

        _containerOptions = containerOptions;
        _databaseOptions = databaseOptions;
        _containerFactory = containerFactory;
    }
    public async Task<IDatabase> CreateAsync(CancellationToken ctx = default)
    {
        // Assumption is db lifecycle is not context bound, request for a database will always start it
        PostgresContainerDatabase db = new(
                _containerOptions,
                _databaseOptions,
                _containerFactory);

        await db.StartAsync(ctx);

        return db;
    }
}
