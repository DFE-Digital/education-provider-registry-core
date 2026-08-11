using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours;

public sealed class PartialSearchBehaviourTests
{
    [Fact]
    public void Name_ReturnsPartial()
    {
        // arrange
        PartialSearchBehaviour<TestEntity> behaviour = new();

        // act
        string result = behaviour.Name;

        // assert
        Assert.Equal("partial", result);
    }

    [Fact]
    public void Build_ReturnsLikeSpecification()
    {
        // arrange
        PartialSearchBehaviour<TestEntity> behaviour = new();

        // act
        ISpecification<TestEntity> result =
            behaviour.Build("Name", "Bob");

        // assert
        Assert.IsType<LikeSpecification<TestEntity>>(result);
    }
}
