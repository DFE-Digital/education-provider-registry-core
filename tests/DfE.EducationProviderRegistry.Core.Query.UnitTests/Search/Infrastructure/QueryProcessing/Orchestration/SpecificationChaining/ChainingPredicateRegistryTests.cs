using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;

public sealed class ChainingPredicateRegistryTests
{
    [Fact]
    public void Resolve_ReturnsCombiner_WhenPredicateExists()
    {
        // arrange
        Func<ISpecification<TestEntity>,
            ISpecification<TestEntity>,
            ISpecification<TestEntity>> combiner =
            (left, right) =>
                new SpecificationStub<TestEntity>(e =>
                    (left?.IsSatisfiedBy(e) ?? true) &&
                    right.IsSatisfiedBy(e));

        Dictionary<string, Func<
            ISpecification<TestEntity>,
            ISpecification<TestEntity>,
            ISpecification<TestEntity>>> map = new()
        {
            { "AND", combiner }
        };

        ChainingPredicateRegistry<TestEntity> registry = new(map);

        // act
        Func<
            ISpecification<TestEntity>,
            ISpecification<TestEntity>,
            ISpecification<TestEntity>> result =
                registry.Resolve("AND");

        // assert
        Assert.Same(combiner, result);
    }

    [Fact]
    public void Resolve_Throws_WhenPredicateNotFound()
    {
        // arrange
        ChainingPredicateRegistry<TestEntity> registry = new([]);

        // act/assert
        Assert.Throws<InvalidOperationException>(() =>
            registry.Resolve("Unknown"));
    }

    [Fact]
    public void Resolve_ReturnedCombiner_CombinesSpecificationsCorrectly()
    {
        // arrange
        SpecificationStub<TestEntity> leftSpec = new(entity => entity.Name == "Bob");
        SpecificationStub<TestEntity> rightSpec = new(entity => entity.Age == 30);

        Func<ISpecification<TestEntity>, ISpecification<TestEntity>, ISpecification<TestEntity>> combiner =
            (left, right) =>
                new SpecificationStub<TestEntity>(entity =>
                    left.IsSatisfiedBy(entity) &&
                    right.IsSatisfiedBy(entity));

        Dictionary<string, Func<
            ISpecification<TestEntity>,
            ISpecification<TestEntity>,
            ISpecification<TestEntity>>> map = new()
        {
            { "AND", combiner }
        };

        ChainingPredicateRegistry<TestEntity> registry = new(map);

        // act
        Func<
            ISpecification<TestEntity>,
            ISpecification<TestEntity>,
            ISpecification<TestEntity>> result =
                registry.Resolve("AND");

        ISpecification<TestEntity> combinedSpec = result(leftSpec, rightSpec);

        // assert
        Assert.True(combinedSpec.IsSatisfiedBy(new TestEntity { Name = "Bob", Age = 30 }));
        Assert.False(combinedSpec.IsSatisfiedBy(new TestEntity { Name = "Bob", Age = 20 }));
        Assert.False(combinedSpec.IsSatisfiedBy(new TestEntity { Name = "Alice", Age = 30 }));
    }
}
