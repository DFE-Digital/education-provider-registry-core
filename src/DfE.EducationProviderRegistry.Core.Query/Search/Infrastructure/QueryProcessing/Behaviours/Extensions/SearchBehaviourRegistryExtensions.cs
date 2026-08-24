using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Extensions;

public static class SearchBehaviourRegistryExtensions
{
    public static ISpecification<TEntity> ResolveBehaviourSpec<TEntity>(
        this ISearchBehaviourRegistry<TEntity> registry,
        string behaviourName,
        string fieldName,
        string value)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(registry);

        ISearchBehaviour<TEntity> behaviour = registry.Get(behaviourName);
        return behaviour.Build(fieldName, value);
    }
}
