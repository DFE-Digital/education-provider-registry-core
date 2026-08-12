using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;

/// <summary>
/// Defines a component capable of executing raw SQL queries that return
/// primary key values for <typeparamref name="TProjection"/> entities.
/// </summary>
/// <typeparam name="TProjection">
/// The EF‑mapped entity or projection type being queried.
/// </typeparam>
public interface ISqlExecutor<TProjection>
{
    /// <summary>
    /// Executes a raw SQL query and returns the primary key values for the
    /// matching <typeparamref name="TProjection"/> entities.
    /// </summary>
    /// <param name="db">The EF Core <see cref="DbContext"/> used to execute the SQL.</param>
    /// <param name="sql">The raw SQL query to execute.</param>
    /// <param name="primaryKeyPropertyName">
    /// The CLR property name of the primary key whose values should be extracted.
    /// </param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>
    /// A list of primary key values extracted from the SQL result set.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="db"/> or <paramref name="sql"/> is <c>null</c>.
    /// </exception>
    Task<List<object>> ExecuteIdsAsync(
        DbContext db,
        string sql,
        string primaryKeyPropertyName,
        CancellationToken cancellationToken = default);
}
