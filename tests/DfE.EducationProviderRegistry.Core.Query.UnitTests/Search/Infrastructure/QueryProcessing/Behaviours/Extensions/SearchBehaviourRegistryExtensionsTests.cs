using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Extensions;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Extensions;

public sealed class SearchBehaviourRegistryExtensionsTests
{
    [Fact]
    public void ResolveBehaviourSpec_Throws_WhenRegistryIsNull()
    {
        // arrange
        ISearchBehaviourRegistry<TestEntity>? registry = null;

        // act/assert
        Assert.Throws<ArgumentNullException>(() =>
            registry!.ResolveBehaviourSpec("Equals", "Name", "Bob"));
    }

    [Fact]
    public void ResolveBehaviourSpec_ResolvesBehaviour_AndBuildsSpecification()
    {
        // arrange
        ISpecification<TestEntity> expectedSpec =
            new SpecificationStub<TestEntity>(e => e.Name == "Bob");

        Mock<ISearchBehaviour<TestEntity>> behaviourMock =
            SearchBehaviourTestDouble.Create("Equals", expectedSpec);

        Mock<ISearchBehaviourRegistry<TestEntity>> registryMock =
            SearchBehaviourRegistryTestDouble.CreateSingle<TestEntity>(
                "Equals",
                behaviourMock.Object);

        // act
        ISpecification<TestEntity> result =
            registryMock.Object.ResolveBehaviourSpec("Equals", "Name", "Bob");

        // assert
        Assert.Same(expectedSpec, result);

        registryMock.Verify(searchBehaviourRegistry =>
            searchBehaviourRegistry.Get("Equals"), Times.Once);
        behaviourMock.Verify(searchBehaviour =>
            searchBehaviour.Build("Name", "Bob"), Times.Once);
    }

    [Fact]
    public void ResolveBehaviourSpec_ReturnsSpecificationBuiltByBehaviour()
    {
        // arrange
        ISpecification<TestEntity> spec =
            new SpecificationStub<TestEntity>(e => e.Name == "Bob");

        Mock<ISearchBehaviour<TestEntity>> behaviourMock =
            SearchBehaviourTestDouble.Create("Equals", spec);

        Mock<ISearchBehaviourRegistry<TestEntity>> registryMock =
            SearchBehaviourRegistryTestDouble.CreateSingle<TestEntity>(
                "Equals",
                behaviourMock.Object);

        // act
        ISpecification<TestEntity> result =
            registryMock.Object.ResolveBehaviourSpec("Equals", "Name", "Bob");

        // assert
        Assert.True(result.IsSatisfiedBy(new TestEntity { Name = "Bob" }));
        Assert.False(result.IsSatisfiedBy(new TestEntity { Name = "Alice" }));
    }
}
