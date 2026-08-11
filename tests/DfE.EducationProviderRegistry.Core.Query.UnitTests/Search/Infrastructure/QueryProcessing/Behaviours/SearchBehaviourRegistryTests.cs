using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours;

public sealed class SearchBehaviourRegistryTests
{
    [Fact]
    public void Get_RegisteredBehaviour_ReturnsExpectedBehaviour()
    {
        // arrange
        ExactSearchBehaviour<TestEntity> behaviour = new();

        SearchBehaviourRegistry<TestEntity> registry =
            new([behaviour]);

        // act
        ISearchBehaviour<TestEntity> result = registry.Get("exact");

        // assert
        Assert.Same(behaviour, result);
    }

    [Fact]
    public void Get_DifferentCase_ReturnsExpectedBehaviour()
    {
        // arrange
        ExactSearchBehaviour<TestEntity> behaviour = new();

        SearchBehaviourRegistry<TestEntity> registry =
            new([behaviour]);

        // act
        ISearchBehaviour<TestEntity> result = registry.Get("EXACT");

        // assert
        Assert.Same(behaviour, result);
    }

    [Fact]
    public void Get_UnknownBehaviour_Throws()
    {
        // arrange
        SearchBehaviourRegistry<TestEntity> registry =
            new([new ExactSearchBehaviour<TestEntity>()]);

        // act / assert
        Assert.Throws<KeyNotFoundException>(() =>
            registry.Get("unknown"));
    }

    [Fact]
    public void Constructor_DuplicateBehaviourNames_Throws()
    {
        // arrange
        ExactSearchBehaviour<TestEntity> first = new();
        ExactSearchBehaviour<TestEntity> second = new();

        // act / assert
        Assert.Throws<ArgumentException>(() =>
            new SearchBehaviourRegistry<TestEntity>([first, second]));
    }
}
