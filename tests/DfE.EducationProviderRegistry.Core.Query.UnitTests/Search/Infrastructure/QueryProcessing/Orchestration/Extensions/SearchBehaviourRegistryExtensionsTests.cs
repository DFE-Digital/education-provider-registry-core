using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration.Extensions;

public sealed class SearchBehaviourRegistryExtensionsTests
{
    [Fact]
    public void ResolveBehaviourSpec_NullRegistry_Throws()
    {
        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            SearchBehaviourRegistryExtensions.ResolveBehaviourSpec<TestEntity>(
                null!,
                "exact",
                "Name",
                "Bob"));
    }

    [Fact]
    public void ResolveBehaviourSpec_RegisteredBehaviour_ReturnsExpectedSpecification()
    {
        // arrange
        SearchBehaviourRegistry<TestEntity> registry =
            new([
                new ExactSearchBehaviour<TestEntity>()
            ]);

        // act
        ISpecification<TestEntity> result =
            registry.ResolveBehaviourSpec(
                "exact",
                "Name",
                "Bob");

        // assert
        Assert.IsType<PropertyEqualsSpecification<TestEntity>>(result);
    }

    [Fact]
    public void ResolveBehaviourSpec_UnknownBehaviour_Throws()
    {
        // arrange
        SearchBehaviourRegistry<TestEntity> registry =
            new([
                new ExactSearchBehaviour<TestEntity>()
            ]);

        // act / assert
        Assert.Throws<KeyNotFoundException>(() =>
            registry.ResolveBehaviourSpec(
                "unknown",
                "Name",
                "Bob"));
    }
}
