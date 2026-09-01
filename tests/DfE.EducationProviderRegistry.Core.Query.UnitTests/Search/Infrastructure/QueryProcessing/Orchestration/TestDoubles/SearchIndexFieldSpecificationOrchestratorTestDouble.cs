using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration.TestDoubles;

public static class SearchIndexFieldSpecificationOrchestratorTestDouble
{
    public static Mock<ISearchIndexFieldSpecificationOrchestrator<TEntity>> Mock<TEntity>()
        where TEntity : class =>
        new(MockBehavior.Strict);

    private static void SetupField<TEntity>(
        Mock<ISearchIndexFieldSpecificationOrchestrator<TEntity>> mock,
        string fieldName,
        ISpecification<TEntity> specification)
        where TEntity : class
    {
        mock.Setup(orchestrator =>
            orchestrator.Orchestrate(
                fieldName,
                It.IsAny<IEnumerable<(string BehaviourName, string? BehaviourPredicate)>>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(specification)
            .Verifiable();
    }

    public static Mock<ISearchIndexFieldSpecificationOrchestrator<TEntity>> Create<TEntity>(
        string fieldName,
        ISpecification<TEntity> specification)
        where TEntity : class
    {
        Mock<ISearchIndexFieldSpecificationOrchestrator<TEntity>> mock = Mock<TEntity>();
        SetupField(mock, fieldName, specification);
        return mock;
    }

    public static Mock<ISearchIndexFieldSpecificationOrchestrator<TEntity>> Create<TEntity>(
        Dictionary<string, ISpecification<TEntity>> fieldSpecs)
        where TEntity : class
    {
        Mock<ISearchIndexFieldSpecificationOrchestrator<TEntity>> mock = Mock<TEntity>();

        foreach (KeyValuePair<string, ISpecification<TEntity>> kvp in fieldSpecs)
        {
            SetupField(mock, kvp.Key, kvp.Value);
        }

        return mock;
    }
}

