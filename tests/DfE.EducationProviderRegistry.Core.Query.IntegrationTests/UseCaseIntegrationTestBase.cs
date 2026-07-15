using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observor;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Observor.Postgres;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests;

public abstract class UseCaseIntegrationTestBase : IntegrationTestBase, IAsyncLifetime
{
    private readonly IDatabaseFactory _databaseFactory;

    protected IDatabase? Database { get; private set; }
#nullable disable
    internal IEstablishmentFactory EstablishmentFactory { get; private set; }
    internal ISearchEstablishmentFactory SearchEstablishmentFactory { get; private set; }
    protected IObservationHandler<PostgresQueryObservations> QueryObservationHandler { get; private set; }
#nullable enable

    protected UseCaseIntegrationTestBase(IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
        _databaseFactory = TestServicesProvider.GetRequiredService<IDatabaseFactory>();
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
        Database = await _databaseFactory.CreateAsync(ct);

        string connectionString = Database.ConnectionString;

        EstablishmentFactory = new EstablishmentFactory(CreateDbContext(connectionString));
        SearchEstablishmentFactory = new SearchEstablishmentFactory(CreateDbContext(connectionString))!;
        QueryObservationHandler = new PostgresQueryObservationHandler(connectionString);

        await Database.StartAsync(ct);
    }

    protected override Task<IConfiguration> GetApplicationConfigurationAsync()
    {
        // TODO options from application to set DatabaseConnection
        return Task.FromResult<IConfiguration>(
            ConfigurationDefault
                .CreateBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>()
                {
                    ["eprweb_eprdat_dotnet_db_connection"] = Database!.ConnectionString
                }).Build());
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
