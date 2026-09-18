using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Test.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests;

public abstract class UseCaseIntegrationTestBase : ServiceProviderTestsBase, IAsyncLifetime
{
#nullable enable
    protected UseCaseIntegrationTestBase(IServiceProvider testServicesProvider)
        : base(testServicesProvider)
    {
        DatabaseFixture = TestServicesProvider.GetRequiredService<EducationProviderRegistryDatabaseFixture>();
    }

#nullable disable
    protected EducationProviderRegistryDatabaseFixture DatabaseFixture { get; }

    public async ValueTask InitializeAsync()
    {
        await StartTestAsync(ct: TestContext.Current.CancellationToken);
    }

    protected sealed override async Task StartTestDependenciesAsync(CancellationToken ct = default)
    {
        await DatabaseFixture.StartAsync(key: "postgres", ct);
    }

    protected sealed override async Task<IConfiguration> BuildApplicationConfigurationAsync()
    {
        return
            new ConfigurationBuilder()
                .AddPostgresConnection(DatabaseFixture.ConnectionString)
                .Build();
    }

    protected Task<UseCaseResponse<TModel>> ExecuteUseCase<TRequest, TModel>(TRequest request, CancellationToken ct = default)
        where TRequest : IUseCaseRequest<UseCaseResponse<TModel>>
    {
        return RunScopedAsync<
            IUseCase<TRequest, UseCaseResponse<TModel>>,
            UseCaseResponse<TModel>>(
                (usecase) => usecase.HandleRequestAsync(request, ct));
    }

    protected override async Task BeforeDisposeAsync()
    {
        await DatabaseFixture.DisposeAsync();
    }
}
