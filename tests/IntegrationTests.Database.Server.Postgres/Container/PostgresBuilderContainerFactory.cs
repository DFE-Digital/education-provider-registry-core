using DotNet.Testcontainers.Containers;
using IntegrationTests.Database.Server.Postgres.Container.Extensions;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Database.Server.Postgres.Container;

internal sealed class PostgresBuilderContainerFactory : IContainerFactory
{
    private readonly PostgresDatabaseOptions _dbOptions;
    private readonly PostgresContainerOptions _containerOptions;

    public PostgresBuilderContainerFactory(PostgresDatabaseOptions dbOptions, PostgresContainerOptions containerOptions)
    {
        _dbOptions = dbOptions;
        _containerOptions = containerOptions;
    }

    public IContainer Create()
    {
        // Important builder is immuteable so each configuration will create a new instance with configuration applied
        PostgreSqlBuilder builder =
            new PostgreSqlBuilder(_containerOptions.Image)
                .WithDatabase(_dbOptions.Database)
                .WithUsername(_dbOptions.Username)
                .WithPassword(_dbOptions.Password)
                .WithExposedPorts(_containerOptions)
                .WithStartupCommands(_containerOptions)
                // Add files that need to be copied into the container before it starts e.g. .sql files to be applied at startup
                .WithMountedResources(_containerOptions.CopyBeforeContainerInit ?? [])
                // forces fresh container state - no mounted volume reuse
                .WithCleanUp(true);

        return builder.Build();
    }
}
