using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Filtering;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;

public interface ISearchProvider<TEntity>
{
    Task<IReadOnlyList<TEntity>> GetMatchingIdsAsync(
        string searchTerm,
        int pageSize,
        int offset,
        IReadOnlyList<SearchFilterRequest> filters,
        CancellationToken cancellationToken = default);
}
