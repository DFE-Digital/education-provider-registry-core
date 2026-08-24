using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing;

public interface ISearchQueryProcessor<TEntity> where TEntity : class
{
    IQueryable<TEntity> ProcessSearch(
        IQueryable<TEntity> query,
        IEnumerable<SearchTerm?>? searchTerms);
}
