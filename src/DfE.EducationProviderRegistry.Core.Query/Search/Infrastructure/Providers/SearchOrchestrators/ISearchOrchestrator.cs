using Microsoft.EntityFrameworkCore;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;

public interface ISearchOrchestrator<TProjection>
    where TProjection : class
{
    Task<IReadOnlyList<TProjection>> ExecuteAsync(
        DbContext db,
        IQueryable<TProjection> baseQuery,
        SearchOrchestratorContext context,
        string searchFilters = "",
        CancellationToken cancellationToken = default);
}
