namespace IntegrationTests.Database.Client.Abstractions;

public interface IDatabaseSession
{
    void Add<TEntity>(TEntity entity) where TEntity : class;
    void AddRange(IEnumerable<object> entities);
    void Update<TEntity>(TEntity entity) where TEntity : class;
    IQueryable<TEntity> Query<TEntity>() where TEntity : class;
}
