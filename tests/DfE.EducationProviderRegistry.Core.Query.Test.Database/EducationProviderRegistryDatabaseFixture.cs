using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Observer;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Observer.Postgres;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database;

public sealed class EducationProviderRegistryDatabaseFixture : IAsyncDisposable
{
    private string? _connectionString;
    private readonly IServiceProvider _provider;
    private readonly IPostgresDatabaseProvider _postgresDatabaseProvider;

    public EducationProviderRegistryDatabaseFixture(
        IServiceProvider provider,
        IPostgresDatabaseProvider postgresDatabaseProvider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(postgresDatabaseProvider);

        _provider = provider;
        _postgresDatabaseProvider = postgresDatabaseProvider;
    }

    public string ConnectionString
    {
        get => _connectionString ?? throw new InvalidOperationException("Database has not been started");
    }

    public IDatabase? Database { get; private set; }
    public IObservationCollector<PostgresQueries>? QueryCollector { get; private set; }

    public async Task StartAsync(string key = "postgres", CancellationToken ct = default)
    {
        Database =
            await _postgresDatabaseProvider.GetDatabaseAsync(
                key: key,
                cancellationToken: ct);

        _connectionString =
            await _postgresDatabaseProvider.GetConnectionStringAsync(
                key: key,
                cancellationToken: ct);

        QueryCollector =
            new PostgresQueryCollector(_connectionString);

        await Database.StartAsync(ct);
    }

    public async Task<TOutput> SeedAsync<TModel, TOutput>(TModel input, CancellationToken ct)
    {
        EnsureDatabaseStarted();

        ISeedDataHandler<TModel, TOutput> handler =
            _provider.GetRequiredService<ISeedDataHandler<TModel, TOutput>>();

        using EducationProviderRegistryDbContext dbContext = CreateDbContext();

        TOutput output =
            await handler.PersistAsync(
                input,
                dbContext,
                ct);

        return output;
    }

    private EducationProviderRegistryDbContext CreateDbContext()
    {
        EnsureDatabaseStarted();

        DbContextOptionsBuilder<EducationProviderRegistryDbContext> contextOptionsBuilder = new();

        contextOptionsBuilder
            .UseNpgsql(_connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();

        EducationProviderRegistryDbContext dbContext = new(contextOptionsBuilder.Options);

        return dbContext;
    }

    private void EnsureDatabaseStarted()
    {
        if (Database is null)
        {
            throw new InvalidOperationException($"Database has not been started with {nameof(StartAsync)}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (Database is not null)
        {
            await Database.DisposeAsync();
        }
    }
}
