using DotNet.Testcontainers.Configurations;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Database.Server.Postgres.Container.Extensions;

internal static class PostgresSqlBuilderExtensions
{
    internal static PostgreSqlBuilder WithExposedPorts(this PostgreSqlBuilder builder, PostgresContainerOptions containerOptions)
    {
        builder = builder.WithExposedPort(PostgresContainerDatabase.PostgresPort);

        bool useStaticHostPort = containerOptions.PublicPort.HasValue;

        return useStaticHostPort ?
            builder.WithPortBinding(
                hostPort: containerOptions.PublicPort!.Value,
                containerPort: PostgresContainerDatabase.PostgresPort)
            :
            builder.WithPortBinding(
                port: PostgresContainerDatabase.PostgresPort,
                assignRandomHostPort: true);
    }

    internal static PostgreSqlBuilder WithStartupCommands(this PostgreSqlBuilder builder, PostgresContainerOptions options)
    {
        string[] flattenedArgs = options.ServerArgs
            .SelectMany((kv)
                => kv.Value.SelectMany((value)
                    => new[]
                    {
                        kv.Key, // e.g. -c
                        value.Trim() // "arg"
                    }))
            .ToArray();

        return builder.WithCommand(flattenedArgs);
    }

    internal static PostgreSqlBuilder WithMountedResources(this PostgreSqlBuilder builder, IEnumerable<ContainerResourceMapping> resources)
    {
        foreach (ContainerResourceMapping resource in resources)
        {
            builder = builder.WithResourceMapping(
                source: resource.Source,
                target: resource.Destination,
                fileMode: GetFileMode(resource));
        }

        return builder;

        static UnixFileModes GetFileMode(ContainerResourceMapping resource)
        {
            UnixFileModes mode =
                UnixFileModes.UserRead |
                UnixFileModes.GroupRead |
                UnixFileModes.OtherRead;

            if (!resource.ReadOnly)
            {
                mode |= UnixFileModes.UserWrite;
            }

            if (resource.Executable)
            {
                mode |= UnixFileModes.UserExecute;
            }

            return mode;
        }
    }
}
