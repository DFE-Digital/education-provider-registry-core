using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;

public interface ISqlExecutor<TProjection>
{
    Task<List<object>> ExecuteIdsAsync(
        DbContext db,
        string sql,
        string primaryKeyPropertyName,
        CancellationToken cancellationToken = default);
}
