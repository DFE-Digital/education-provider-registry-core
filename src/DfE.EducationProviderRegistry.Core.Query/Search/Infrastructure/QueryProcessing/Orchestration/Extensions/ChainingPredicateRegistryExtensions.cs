using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.Extensions;

public static class ChainingPredicateRegistryExtensions
{
    public static ISpecification<TEntity> Chain<TEntity>(
        this IChainingPredicateRegistry<TEntity> registry,
        ISpecification<TEntity>? left,
        ISpecification<TEntity> right,
        string? predicateName)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(registry);

        if (string.IsNullOrWhiteSpace(predicateName))
        {
            return right;
        }

        Func<ISpecification<TEntity>,
            ISpecification<TEntity>,
            ISpecification<TEntity>> combiner =
            registry.Resolve(predicateName);

        return left is null ? right : combiner(left, right);
    }
}

