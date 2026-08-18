using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration.TestDoubles;

public static class SearchBehaviourTestDouble
{
    public static Mock<ISearchBehaviour<TEntity>> Mock<TEntity>()
        where TEntity : class =>
        new(MockBehavior.Strict);

    public static Mock<ISearchBehaviour<TEntity>> Create<TEntity>(
        string behaviourName,
        ISpecification<TEntity> specification)
        where TEntity : class
    {
        Mock<ISearchBehaviour<TEntity>> mock = Mock<TEntity>();

        mock.Setup(searchBehaviour =>
            searchBehaviour.Build(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(specification)
            .Verifiable();

        return mock;
    }

    public static Mock<ISearchBehaviour<TEntity>> Create<TEntity>(
        string behaviourName,
        Func<string, string, ISpecification<TEntity>> builder)
        where TEntity : class
    {
        Mock<ISearchBehaviour<TEntity>> mock = Mock<TEntity>();

        mock.Setup(searchBehaviour =>
            searchBehaviour.Build(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string field, string value) => builder(field, value))
            .Verifiable();

        return mock;
    }
}

