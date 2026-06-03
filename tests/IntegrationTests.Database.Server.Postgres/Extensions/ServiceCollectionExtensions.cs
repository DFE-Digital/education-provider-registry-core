using IntegrationTests.Database.Server.Abstractions;
using IntegrationTests.Database.Server.Postgres.Container;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IntegrationTests.Database.Server.Postgres.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddPostgresDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddPostgresOptions(configuration);
        services.AddSingleton<IDatabaseFactory, PostgresDatabaseFactory>();
        services.AddSingleton<IContainerFactory, PostgresBuilderContainerFactory>();
        return services;
    }

    private static IServiceCollection AddPostgresOptions(this IServiceCollection services, IConfiguration configuration)
    {
        // ContainerOptions
        services.AddOptions<PostgresContainerOptions>()
            .Bind(configuration.GetRequiredSection(nameof(PostgresContainerOptions)))
            .Validate(t => !string.IsNullOrWhiteSpace(t.ImageTag), failureMessage: "Image tag cannot be empty")
            .Validate(t => !string.IsNullOrWhiteSpace(t.ImageName), failureMessage: "Image name cannot be empty")
            .ValidateOnStart()
            .RegisterValueFromIOptions();

        // DatabaseOptions
        services.AddOptions<PostgresDatabaseOptions>()
            .Bind(configuration.GetRequiredSection(nameof(PostgresDatabaseOptions)))
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Database), failureMessage: "Database should not be empty")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Username), failureMessage: "Username should not be null or empty")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Password), failureMessage: "Password should not be null or empty")
            .ValidateOnStart()
            .RegisterValueFromIOptions();

        return services;
    }

    private static IServiceCollection RegisterValueFromIOptionsWrapper<TOptionsType>(this IServiceCollection services) where TOptionsType : class
    {
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<TOptionsType>>().Value);
        return services;
    }

    private static IServiceCollection RegisterValueFromIOptions<TOptionsType>(this OptionsBuilder<TOptionsType> builder) where TOptionsType : class
    {
        return builder.Services.RegisterValueFromIOptionsWrapper<TOptionsType>();
    }
}
