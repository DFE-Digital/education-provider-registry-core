using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.Core.Libraries.DesignPatterns.Specification.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Orchestration;

public sealed class SearchTermSpecificationOrchestrator<TEntity> : ISearchTermSpecificationOrchestrator<TEntity> where TEntity : class
{
    private readonly ISearchSpecificationOrchestrator<TEntity> _specificationOrchestrator;

    public SearchTermSpecificationOrchestrator(ISearchSpecificationOrchestrator<TEntity> specificationOrchestrator)
    {
        _specificationOrchestrator = specificationOrchestrator;
    }

    public IQueryable<TEntity> ApplySearch(
        IQueryable<TEntity> query,
        IEnumerable<SearchTerm?>? searchTerms)
    {
        List<SearchTerm> validTerms =
            searchTerms?
                .Where(searchTerm =>
                    searchTerm is not null &&
                    !string.IsNullOrWhiteSpace(searchTerm.Key) &&
                    !string.IsNullOrWhiteSpace(searchTerm.Value))
                .Select(searchTerm => searchTerm!)
                .ToList()
            ?? [];

        if (validTerms.Count == 0)
        {
            return query;
        }

        ISpecification<TEntity>? combined = null;

        foreach (SearchTerm term in validTerms)
        {
            ISpecification<TEntity> spec =
                _specificationOrchestrator.Orchestrate(term.Key, term.Value);

            combined = combined is null
                ? spec
                : combined.And(spec);
        }

        return query.Where(combined!.ToExpression());
    }
}
