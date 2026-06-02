using DotNet.Testcontainers.Containers;
using IntegrationTests.Database.Server.Abstractions;
using Npgsql;

namespace IntegrationTests.Database.Server.Postgres.Container;

internal sealed class PostgresContainerDatabase : IDatabase
{
    public const int PostgresPort = 5432;

    private readonly Lazy<IContainer> _container;
    private readonly PostgresDatabaseOptions _databaseOptions;
    private readonly SemaphoreSlim _startLock = new(1, 1);
    private bool _started;
    private string? _connectionString;

    public PostgresContainerDatabase(
        PostgresContainerOptions containerOptions,
        PostgresDatabaseOptions databaseOptions,
        IContainerFactory containerFactory)
    {
        // DatabaseOptions
        ArgumentNullException.ThrowIfNull(databaseOptions);
        _databaseOptions = databaseOptions;

        // ContainerOptions
        ArgumentNullException.ThrowIfNull(containerOptions);

        ArgumentNullException.ThrowIfNull(containerFactory);
        _container = new Lazy<IContainer>(containerFactory.Create);
    }

    public async ValueTask DisposeAsync()
    {
        if (_container != null)
        {
            await _container.Value.DisposeAsync();
        }
    }


    public string ConnectionString
    {
        get
        {
            if (_connectionString == null)
            {
                throw new InvalidOperationException("Database not started");
            }

            return _connectionString;
        }
    }

    public async Task StartAsync(CancellationToken ctx = default)
    {
        if (_started)
        {
            return;
        }

        await _startLock.WaitAsync(ctx);

        try
        {
            if (_started)
            {
                return;
            }

            await _container.Value.StartAsync(ctx);

            IContainer container = _container.Value;

            _connectionString =
                $"Host={container.Hostname};Port={container.GetMappedPublicPort(PostgresPort)};Database={_databaseOptions.Database};Username={_databaseOptions.Username};Password={_databaseOptions.Password};";

            _started = true;
        }
        finally
        {
            _startLock.Release();
        }
    }

    public async Task ExecuteAsync(string sql, CancellationToken ctx = default)
    {
        await using NpgsqlConnection connection = new(ConnectionString);
        await connection.OpenAsync(ctx);

        await using NpgsqlCommand command = new(sql, connection);
        await command.ExecuteNonQueryAsync(ctx);
    }
}
