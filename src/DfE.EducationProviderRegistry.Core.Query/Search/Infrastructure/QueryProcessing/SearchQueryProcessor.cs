using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing;

public sealed class SearchQueryProcessor<TEntity> : ISearchQueryProcessor<TEntity>
    where TEntity : class
{
    private readonly ISearchTermSpecificationOrchestrator<TEntity> _specificationOrchestrator;
    private readonly IChainingPredicateRegistry<TEntity> _predicateRegistry;


    public SearchQueryProcessor(
        ISearchTermSpecificationOrchestrator<TEntity> specificationOrchestrator,
        IChainingPredicateRegistry<TEntity> predicateRegistry)
    {
        _specificationOrchestrator = specificationOrchestrator;
        _predicateRegistry = predicateRegistry;
    }

    public IQueryable<TEntity> ProcessSearch(
        IQueryable<TEntity> query,
        IEnumerable<SearchTerm> searchTerms)
    {
        ISpecification<TEntity>? combined = null;

        foreach (SearchTerm term in searchTerms ?? [])
        {
            if (term is null)
            {
                continue;
            }

            ISpecification<TEntity> spec =
                _specificationOrchestrator.Orchestrate(term.Key, term.Value);

            combined = _predicateRegistry.Chain(
                combined,
                spec,
                "AND");
        }

        return combined is null
            ? query
            : query.Where(combined.ToExpression());
    }

}
