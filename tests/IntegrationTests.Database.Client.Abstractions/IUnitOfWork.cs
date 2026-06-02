namespace IntegrationTests.Database.Client.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    IDatabaseSession Session { get; }
    Task CommitAsync(CancellationToken ctx = default);
}
