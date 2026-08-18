using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Provider;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observer;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observer.Postgres;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests;

public abstract class UseCaseIntegrationTestBase : IntegrationTestBase, IAsyncLifetime
{
    private readonly IPostgresDatabaseProvider _databaseProvider;
    private readonly PostgresContainerOptions _postgresOptions;

    protected IDatabase? Database { get; private set; }
#nullable disable
    internal ISearchEstablishmentSeeder SeedSearchEstablishments { get; private set; }
    internal IObservationCollector<PostgresQueries> QueryCollector { get; private set; }
#nullable enable
    protected UseCaseIntegrationTestBase(IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
        _databaseProvider = TestServicesProvider.GetRequiredService<IPostgresDatabaseProvider>();
        _postgresOptions = TestServicesProvider.GetRequiredService<IOptionsMonitor<PostgresContainerOptions>>().Get("postgres");
    }

    // Hook called by XUnit to initialise before any tests run
    public async ValueTask InitializeAsync()
    {
        await StartTestAsync(ct: TestContext.Current.CancellationToken);
    }

    protected Task<UseCaseResponse<TModel>> ExecuteUseCase<TRequest, TModel>(TRequest request) where TRequest : IUseCaseRequest<UseCaseResponse<TModel>>
    {
        return RunScopedAsync<
            IUseCase<TRequest, UseCaseResponse<TModel>>,
            UseCaseResponse<TModel>>(
                (usecase) =>
                    usecase.HandleRequestAsync(
                        request,
                        TestContext.Current.CancellationToken));
    }

    protected override async Task StartTestDependenciesAsync(CancellationToken ct = default)
    {
        Database = await _databaseProvider.GetDatabaseAsync("postgres", ct);

        string connectionString = await GetDatabaseConnectionString(Database, _postgresOptions.Database!);

        SeedSearchEstablishments = new SearchEstablishmentSeeder(CreateDbContext(connectionString))!;
        QueryCollector = new PostgresQueryCollector(connectionString);
        await Database.StartAsync(ct);
    }

    protected override async Task<IConfiguration> GetApplicationConfigurationAsync()
    {
        // TODO options from application to set DatabaseConnection
        string connectionString = await GetDatabaseConnectionString(Database!, _postgresOptions.Database!);

        return
            ConfigurationDefault
                .CreateBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>()
                {
                    ["eprweb_eprdat_dotnet_db_connection"] = connectionString
                }).Build();
    }

    protected override async Task BeforeDisposeAsync()
    {
        if (Database != null)
        {
            await Database.DisposeAsync();
        }
    }

    private static EducationProviderRegistryDbContext CreateDbContext(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        DbContextOptionsBuilder<EducationProviderRegistryDbContext> contextOptionsBuilder = new();
        contextOptionsBuilder
            .UseNpgsql(connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();
        EducationProviderRegistryDbContext dbContext = new(contextOptionsBuilder.Options);
        return dbContext;
    }

    private static async Task<string> GetDatabaseConnectionString(IDatabase db, PostgresDatabaseOptions dbOptions)
    {
        DatabaseEndpoint endpoint = db.GetDatabaseEndpoint();

        NpgsqlConnectionStringBuilder builder = new()
        {
            Host = endpoint.Host,
            Port = endpoint.Port,
            Database = dbOptions.Name,
            Username = dbOptions.Username,
            Password = dbOptions.Password
        };

        return builder.ToString();
    }
}
