using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.Extensions;

public static class SearchBehaviourRegistryExtensions
{
    public static ISpecification<TEntity> ResolveBehaviourSpec<TEntity>(
        this SearchBehaviourRegistry<TEntity> registry,
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
