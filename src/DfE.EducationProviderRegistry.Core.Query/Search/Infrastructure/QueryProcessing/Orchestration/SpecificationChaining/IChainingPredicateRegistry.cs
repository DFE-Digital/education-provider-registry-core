using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;

public interface IChainingPredicateRegistry<TEntity> where TEntity : class
{
    Func<
        ISpecification<TEntity>,
        ISpecification<TEntity>,
        ISpecification<TEntity>> Resolve(string name);
}
