using DotNet.Testcontainers.Containers;

namespace IntegrationTests.Database.Server.Postgres.Container;

public interface IContainerFactory
{
    IContainer Create();
}
