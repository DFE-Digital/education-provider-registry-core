namespace IntegrationTests.Database.Client.Abstractions;

public interface IUnitOfWorkFactory
{
    Task<IUnitOfWork> CreateAsync(CancellationToken ctx = default);
}
