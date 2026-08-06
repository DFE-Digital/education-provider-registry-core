using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;

public sealed class ChainingPredicateRegistry<TEntity>
    where TEntity : class
{
    private readonly Dictionary<
        string,
        Func<ISpecification<TEntity>, ISpecification<TEntity>, ISpecification<TEntity>>> _map;

    public ChainingPredicateRegistry(
        Dictionary<string,
            Func<ISpecification<TEntity>, ISpecification<TEntity>, ISpecification<TEntity>>> map)
    {
        _map = map;
    }

    public Func<ISpecification<TEntity>, ISpecification<TEntity>, ISpecification<TEntity>> Resolve(string name)
    {
        if (!_map.TryGetValue(name,
                out Func<ISpecification<TEntity>, ISpecification<TEntity>, ISpecification<TEntity>>? combiner))
            throw new InvalidOperationException($"Unknown chaining predicate '{name}'.");

        return combiner;
    }
}

