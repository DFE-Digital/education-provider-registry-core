using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;

[ExcludeFromCodeCoverage]
internal sealed class SqlExecutor<TProjection> : ISqlExecutor<TProjection>
    where TProjection : class
{
    public async Task<List<object>> ExecuteIdsAsync(
        DbContext db,
        string sql,
        string primaryKeyPropertyName,
        object[] parameters,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(db);
        ArgumentNullException.ThrowIfNull(sql);

        return await db.Set<TProjection>()
            .FromSqlRaw(sql, parameters)
            .Select(projection =>
                EF.Property<object>(projection, primaryKeyPropertyName))
            .ToListAsync(cancellationToken);
    }
}
