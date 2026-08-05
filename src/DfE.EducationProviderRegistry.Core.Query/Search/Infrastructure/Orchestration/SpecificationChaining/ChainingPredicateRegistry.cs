using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Orchestration.SpecificationChaining;

public sealed class ChainingPredicateRegistry<TEntity>
    where TEntity : class
{
    private readonly Dictionary<
        string, Func<ISpecification<TEntity>, ISpecification<TEntity>, ISpecification<TEntity>>> _map;

    public ChainingPredicateRegistry()
    {
        _map = new(StringComparer.OrdinalIgnoreCase);
    }

    public void Register(
        string name,
        Func<ISpecification<TEntity>, ISpecification<TEntity>, ISpecification<TEntity>> combiner)
    {
        _map[name] = combiner;
    }

    public Func<ISpecification<TEntity>, ISpecification<TEntity>, ISpecification<TEntity>> Resolve(string? name)
    {
        if (name is null)
            throw new InvalidOperationException("No chaining predicate was provided.");

        if (!_map.TryGetValue(name, out Func<ISpecification<TEntity>, ISpecification<TEntity>, ISpecification<TEntity>>? combiner))
            throw new InvalidOperationException($"Unknown chaining predicate '{name}'.");

        return combiner;
    }
}
