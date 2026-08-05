using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Orchestration;

public interface ISearchTermSpecificationOrchestrator
{
    IQueryable<TEntity> ApplySearch<TEntity>(
        IQueryable<TEntity> query,
        IEnumerable<SearchTerm?>? searchTerms) where TEntity : class;
}
