namespace IntegrationTests.Database.Server.Abstractions;

public interface IDatabaseFactory
{
    Task<IDatabase> CreateAsync(CancellationToken ctx = default);
}
