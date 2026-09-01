using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observer;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observer.Postgres;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests;

public abstract class UseCaseIntegrationTestBase : ServiceProviderTestsBase, IAsyncLifetime
{
    private readonly IPostgresDatabaseProvider _databaseProvider;
    private string? _postgresLocalConnectionString;

#nullable enable
    protected UseCaseIntegrationTestBase(IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
        _databaseProvider = TestServicesProvider.GetRequiredService<IPostgresDatabaseProvider>();
    }

    protected IDatabase? Database { get; private set; }
#nullable disable
    internal ISearchEstablishmentSeeder SeedSearchEstablishments { get; private set; }
    internal IObservationCollector<PostgresQueries> QueryCollector { get; private set; }

    // Hook called by XUnit to initialise before any tests run
    public async ValueTask InitializeAsync()
    {
        await StartTestAsync(ct: TestContext.Current.CancellationToken);
    }

    protected sealed override async Task StartTestDependenciesAsync(CancellationToken ct = default)
    {
        const string PostgresContainerKey = "postgres";

        Database = await _databaseProvider.GetDatabaseAsync(key: PostgresContainerKey, ct);
        _postgresLocalConnectionString = await _databaseProvider.GetConnectionStringAsync(key: PostgresContainerKey, cancellationToken: ct);

        SeedSearchEstablishments = new SearchEstablishmentSeeder(CreateDbContext(_postgresLocalConnectionString))!;
        QueryCollector = new PostgresQueryCollector(_postgresLocalConnectionString);
        await Database.StartAsync(ct);
    }

    protected sealed override async Task<IConfiguration> BuildApplicationConfigurationAsync()
    {
        return
            ConfigurationDefault
                .CreateBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    ["eprweb_eprdat_dotnet_db_connection"] = _postgresLocalConnectionString
                }).Build();
    }

    protected Task<UseCaseResponse<TModel>> ExecuteUseCase<TRequest, TModel>(TRequest request) where TRequest : IUseCaseRequest<UseCaseResponse<TModel>>
    {
        return RunScopedAsync<
            IUseCase<TRequest, UseCaseResponse<TModel>>,
            UseCaseResponse<TModel>>(
                (usecase) => usecase.HandleRequestAsync(request, TestContext.Current.CancellationToken));
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
}
