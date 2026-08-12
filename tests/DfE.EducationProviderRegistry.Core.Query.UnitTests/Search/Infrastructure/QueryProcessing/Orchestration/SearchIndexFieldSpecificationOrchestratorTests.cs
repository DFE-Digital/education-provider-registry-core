using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration;

public sealed class SearchIndexFieldSpecificationOrchestratorTests
{
    [Fact]
    public void Orchestrate_Throws_WhenBehavioursIsNull()
    {
        // arrange
        Mock<ISearchBehaviourRegistry<TestEntity>> behaviourRegistryMock =
            SearchBehaviourRegistryTestDouble.Mock<TestEntity>();

        Mock<IChainingPredicateRegistry<TestEntity>> predicateRegistryMock =
            ChainingPredicateRegistryTestDouble.Mock<TestEntity>();

        SearchIndexFieldSpecificationOrchestrator<TestEntity> orchestrator =
            new(
                behaviourRegistryMock.Object,
                predicateRegistryMock.Object);

        // act & assert
        Assert.Throws<ArgumentNullException>(() =>
            orchestrator.Orchestrate("Name", null!, "AND", "Bob"));
    }

    [Fact]
    public void Orchestrate_Throws_WhenNoBehavioursConfigured()
    {
        // arrange
        Mock<ISearchBehaviourRegistry<TestEntity>> behaviourRegistryMock =
            SearchBehaviourRegistryTestDouble.Mock<TestEntity>();

        Mock<IChainingPredicateRegistry<TestEntity>> predicateRegistryMock =
            ChainingPredicateRegistryTestDouble.Mock<TestEntity>();

        SearchIndexFieldSpecificationOrchestrator<TestEntity> orchestrator =
            new(
                behaviourRegistryMock.Object,
                predicateRegistryMock.Object);

        List<(string BehaviourName, string? BehaviourPredicate)> behaviours = [];

        // act & assert
        Assert.Throws<InvalidOperationException>(() =>
            orchestrator.Orchestrate("Name", behaviours, "AND", "Bob"));
    }

    [Fact]
    public void Orchestrate_CallsBehaviourRegistry_ForEachBehaviour()
    {
        // arrange
        ISpecification<TestEntity> spec1 =
            new SpecificationStub<TestEntity>(entity => entity.Name == "Bob");

        ISpecification<TestEntity> spec2 =
            new SpecificationStub<TestEntity>(entity => entity.Age == 30);

        Mock<ISearchBehaviour<TestEntity>> behaviour1 =
            SearchBehaviourTestDouble.Create("EqualsName", spec1);

        Mock<ISearchBehaviour<TestEntity>> behaviour2 =
            SearchBehaviourTestDouble.Create("EqualsAge", spec2);

        Mock<ISearchBehaviourRegistry<TestEntity>> behaviourRegistryMock =
            SearchBehaviourRegistryTestDouble.Create<TestEntity>(
                new Dictionary<string, ISearchBehaviour<TestEntity>>
                {
                    { "EqualsName", behaviour1.Object },
                    { "EqualsAge", behaviour2.Object }
                });

        Mock<IChainingPredicateRegistry<TestEntity>> predicateRegistryMock =
            ChainingPredicateRegistryTestDouble.CreateAnd<TestEntity>();

        List<(string BehaviourName, string? BehaviourPredicate)> behaviours =
        [
            ("EqualsName", null),
            ("EqualsAge", null)
        ];

        SearchIndexFieldSpecificationOrchestrator<TestEntity> orchestrator =
            new(
                behaviourRegistryMock.Object,
                predicateRegistryMock.Object);

        // act
        ISpecification<TestEntity> result =
            orchestrator.Orchestrate("Name", behaviours, "AND", "Bob");

        // assert/verify
        Assert.True(result.IsSatisfiedBy(new TestEntity { Name = "Bob", Age = 30 }));
        Assert.False(result.IsSatisfiedBy(new TestEntity { Name = "Bob", Age = 20 }));

        behaviourRegistryMock.Verify(chainingPredicateRegistry =>
            chainingPredicateRegistry.Get("EqualsName"), Times.Once);
        behaviourRegistryMock.Verify(chainingPredicateRegistry =>
            chainingPredicateRegistry.Get("EqualsAge"), Times.Once);
        predicateRegistryMock.Verify(chainingPredicateRegistry =>
            chainingPredicateRegistry.Resolve("AND"), Times.Exactly(2));
    }

    [Fact]
    public void Orchestrate_UsesBehaviourPredicate_WhenProvided()
    {
        // arrange
        ISpecification<TestEntity> spec =
            new SpecificationStub<TestEntity>(entity => entity.Name == "Bob");

        Mock<ISearchBehaviour<TestEntity>> behaviour =
            SearchBehaviourTestDouble.Create("EqualsName", spec);

        Mock<ISearchBehaviourRegistry<TestEntity>> behaviourRegistryMock =
            SearchBehaviourRegistryTestDouble.CreateSingle<TestEntity>(
                "EqualsName",
                behaviour.Object);

        Mock<IChainingPredicateRegistry<TestEntity>> predicateRegistryMock =
            ChainingPredicateRegistryTestDouble.CreateOr<TestEntity>();

        List<(string BehaviourName, string? BehaviourPredicate)> behaviours =
        [
            ("EqualsName", "OR")
        ];

        SearchIndexFieldSpecificationOrchestrator<TestEntity> orchestrator =
            new(
                behaviourRegistryMock.Object,
                predicateRegistryMock.Object);

        // act
        ISpecification<TestEntity> result =
            orchestrator.Orchestrate("Name", behaviours, "AND", "Bob");

        // assert/verify
        Assert.True(result.IsSatisfiedBy(new TestEntity { Name = "Bob" }));
        Assert.False(result.IsSatisfiedBy(new TestEntity { Name = "Alice" }));

        predicateRegistryMock.Verify(chainingPredicateRegistry =>
            chainingPredicateRegistry.Resolve("OR"), Times.Once);
    }
}
