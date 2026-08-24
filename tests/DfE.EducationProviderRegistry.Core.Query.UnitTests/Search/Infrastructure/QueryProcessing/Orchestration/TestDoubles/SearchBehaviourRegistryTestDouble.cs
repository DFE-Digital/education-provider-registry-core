using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration.TestDoubles;

public static class SearchBehaviourRegistryTestDouble
{
    public static Mock<ISearchBehaviourRegistry<TEntity>> Mock<TEntity>()
        where TEntity : class =>
        new(MockBehavior.Strict);

    public static Mock<ISearchBehaviourRegistry<TEntity>> Create<TEntity>(
        Dictionary<string, ISearchBehaviour<TEntity>> behaviours)
        where TEntity : class
    {
        Mock<ISearchBehaviourRegistry<TEntity>> mock = Mock<TEntity>();

        foreach ((string behaviourName, ISearchBehaviour<TEntity> behaviour) in behaviours)
        {
            mock.Setup(searchBehaviourRegistry =>
                searchBehaviourRegistry.Get(behaviourName))
                .Returns(behaviour)
                .Verifiable();
        }

        return mock;
    }

    public static Mock<ISearchBehaviourRegistry<TEntity>> CreateSingle<TEntity>(
        string behaviourName,
        ISearchBehaviour<TEntity> behaviour)
        where TEntity : class =>
        Create<TEntity>(
            new Dictionary<string, ISearchBehaviour<TEntity>>
            {
                { behaviourName, behaviour }
            });
}
