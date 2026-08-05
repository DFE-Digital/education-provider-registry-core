using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Orchestration;

public interface ISearchTermSpecificationOrchestrator<TEntity> where TEntity : class
{
    IQueryable<TEntity> ApplySearch(
        IQueryable<TEntity> query,
        IEnumerable<SearchTerm?>? searchTerms);
}
