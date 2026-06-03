using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Abstractions;

public abstract class IntegrationTestBase : IAsyncDisposable
{
    private IServiceProvider? _applicationServicesRootProvider;
    protected IntegrationTestBase(IServiceProvider testServicesProvider)
    {
        ArgumentNullException.ThrowIfNull(testServicesProvider);
        TestServicesProvider = testServicesProvider;
    }

    protected IServiceProvider TestServicesProvider { get; }

    private IServiceProvider ApplicationServicesRootProvider
    {
        get => _applicationServicesRootProvider ??
            throw new InvalidOperationException("Application services have not been initialised");

        set => _applicationServicesRootProvider = value ??
            throw new ArgumentNullException(nameof(value));
    }

    protected async ValueTask StartTestAsync(CancellationToken ct = default)
    {
        if (_applicationServicesRootProvider is not null)
        {
            throw new InvalidOperationException("Test already started");
        }

        await BeforeStartTestDependenciesAsync(ct);

        await StartTestDependenciesAsync(ct);

        await AfterStartTestDependenciesAsync(ct);

        ApplicationServicesRootProvider =
            BuildApplicationServices(
                configuration: await MergeTestAndApplicationConfiguration(),
                configure: ConfigureApplicationServices);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await BeforeDisposeAsync();

        if (_applicationServicesRootProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (_applicationServicesRootProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

        await AfterDisposeAsync();
    }

    protected virtual Task BeforeStartTestDependenciesAsync(CancellationToken ct = default) => Task.CompletedTask;
    protected virtual Task StartTestDependenciesAsync(CancellationToken ct = default) => Task.CompletedTask;
    protected virtual Task AfterStartTestDependenciesAsync(CancellationToken ct = default) => Task.CompletedTask;
    protected virtual void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration) { }
    protected virtual void ConfigureApplicationConfiguration(IConfigurationBuilder builder) { }
    protected virtual Task<IConfiguration> GetApplicationConfigurationAsync()
        => Task.FromResult(
            IConfigurationFactory.CreateEmpty());

    // BeforeDisposeAsync runs BEFORE application services are disposed.
    // Use this to clean up external resources (e.g., databases, containers).
    protected virtual Task BeforeDisposeAsync() => Task.CompletedTask;
    protected virtual Task AfterDisposeAsync() => Task.CompletedTask;

    protected TSingletonService ResolveSingletonApplicationService<TSingletonService>() where TSingletonService : notnull
    {
        return ApplicationServicesRootProvider.GetRequiredService<TSingletonService>();
    }

    protected async Task<TResult> RunScopedApplicationServicesAsync<TResult>(Func<IServiceProvider, Task<TResult>> action)
    {
        using IServiceScope scope = CreateApplicationServiceScope();
        return await action(scope.ServiceProvider);
    }

    private async Task<IConfiguration> MergeTestAndApplicationConfiguration()
    {
        IConfigurationBuilder builder =
            IConfigurationBuilderFactory.CreateDefault()
        // add test config
            .AddConfiguration(TestServicesProvider.GetRequiredService<IConfiguration>())
        // add app config
            .AddConfiguration(await GetApplicationConfigurationAsync());

        ConfigureApplicationConfiguration(builder);

        return builder.Build();
    }

    private IServiceScope CreateApplicationServiceScope()
    {
        return ApplicationServicesRootProvider.CreateScope();
    }

    private static IServiceProvider BuildApplicationServices(IConfiguration configuration, Action<IServiceCollection, IConfiguration>? configure = null)
    {
        IServiceCollection services = IServiceCollectionFactory.CreateDefault();
        configure?.Invoke(services, configuration);
        services.AddSingleton<IConfiguration>((sp) => configuration);

        IServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions()
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });

        return provider;
    }
}
