using IntegrationTests.Abstractions;
using IntegrationTests.Database.Server.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests;

public abstract class UseCaseIntegrationTestBase : IntegrationTestBase, IAsyncLifetime
{
    private readonly IDatabaseFactory _databaseFactory;

    protected IDatabase? Database { get; private set; }

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

    protected override async Task StartTestDependenciesAsync(CancellationToken ct = default)
    {
        Database = await _databaseFactory.CreateAsync(ct);
        await Database.StartAsync(ct);
    }

    protected override Task<IConfiguration> GetApplicationConfigurationAsync()
    {
        // TODO options from application to set DatabaseConnection
        return Task.FromResult<IConfiguration>(
            DefaultConfigurationBuilder
                .Create()
                .Build());
    }

    protected override async Task BeforeDisposeAsync()
    {
        if (Database != null)
        {
            await Database.DisposeAsync();
        }
    }
}
