using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

public static class SearchTermSpecificationOrchestratorTestDouble
{
    public static Mock<ISearchTermSpecificationOrchestrator<TEntity>> Mock<TEntity>()
        where TEntity: class => new(MockBehavior.Strict);

    public static Mock<ISearchTermSpecificationOrchestrator<TEntity>> Create<TEntity>(
        Dictionary<(string Key, string Value), ISpecification<TEntity>> specifications)
        where TEntity : class
    {
        Mock<ISearchTermSpecificationOrchestrator<TEntity>> mock = Mock<TEntity>();

        mock.Setup(orchestrator =>
            orchestrator.Orchestrate(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string key, string value) =>
            {
                if (specifications.TryGetValue((key, value), out ISpecification<TEntity>? spec))
                {
                    return spec;
                }
                return new SpecificationStub<TEntity>(_ => true);
            })
            .Verifiable();

        return mock;
    }
}

