using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing;

public sealed class SearchQueryProcessor<TEntity> : ISearchQueryProcessor<TEntity>
    where TEntity : class
{
    private readonly ISearchTermSpecificationOrchestrator<TEntity> _specificationOrchestrator;
    private readonly ChainingPredicateRegistry<TEntity> _predicateRegistry;

    public SearchQueryProcessor(
        ISearchTermSpecificationOrchestrator<TEntity> specificationOrchestrator,
        ChainingPredicateRegistry<TEntity> predicateRegistry,
        IOptions<SearchConfiguration> searchConfiguration)
    {
        _specificationOrchestrator = specificationOrchestrator;
        _predicateRegistry = predicateRegistry;
    }

    public IQueryable<TEntity> ProcessSearch(
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

        if (validTerms.Count == 1)
        {
            ISpecification<TEntity> singleSpec =
                _specificationOrchestrator
                    .Orchestrate(validTerms[0].Key, validTerms[0].Value);

            return query.Where(singleSpec.ToExpression());
        }

        Func<ISpecification<TEntity>,
            ISpecification<TEntity>,
            ISpecification<TEntity>> andCombiner =
                _predicateRegistry.Resolve("AND");

        ISpecification<TEntity>? combined = null;

        foreach (SearchTerm term in validTerms)
        {
            ISpecification<TEntity> termSpec =
                _specificationOrchestrator.Orchestrate(term.Key, term.Value);

            combined = combined is null
                ? termSpec
                : andCombiner(combined, termSpec);
        }

        return query.Where(combined!.ToExpression());
    }
}
