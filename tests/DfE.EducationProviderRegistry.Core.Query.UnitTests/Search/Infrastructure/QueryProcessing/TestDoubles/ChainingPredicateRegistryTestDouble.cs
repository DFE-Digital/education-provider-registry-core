using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

public static class ChainingPredicateRegistryTestDouble
{
    public static Mock<IChainingPredicateRegistry<TEntity>> Mock<TEntity>()
        where TEntity : class =>
        new(MockBehavior.Strict);

    private static Mock<IChainingPredicateRegistry<TEntity>> Create<TEntity>(
        string predicateName,
        Func<ISpecification<TEntity>?, ISpecification<TEntity>, ISpecification<TEntity>> combiner)
        where TEntity : class
    {
        Mock<IChainingPredicateRegistry<TEntity>> mock = Mock<TEntity>();

        mock.Setup(registry => registry.Resolve(predicateName))
            .Returns((string _) => combiner)
            .Verifiable();

        return mock;
    }

    public static Mock<IChainingPredicateRegistry<TEntity>> CreateAnd<TEntity>()
        where TEntity : class =>
        Create<TEntity>(
            predicateName: "AND",
            combiner: (left, right) =>
                new SpecificationStub<TEntity>(entity =>
                    (left?.IsSatisfiedBy(entity) ?? true) &&
                    right.IsSatisfiedBy(entity))
        );

    public static Mock<IChainingPredicateRegistry<TEntity>> CreateOr<TEntity>()
        where TEntity : class =>
        Create<TEntity>(
            predicateName: "OR",
            combiner: (left, right) =>
                new SpecificationStub<TEntity>(entity =>
                    (left?.IsSatisfiedBy(entity) ?? false) ||
                    right.IsSatisfiedBy(entity))
        );
}
