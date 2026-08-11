using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours;

public sealed class ExactSearchBehaviourTests
{
    [Fact]
    public void Name_ReturnsExact()
    {
        // arrange
        ExactSearchBehaviour<TestEntity> behaviour = new();

        // act
        string result = behaviour.Name;

        // assert
        Assert.Equal("exact", result);
    }

    [Fact]
    public void Build_ReturnsPropertyEqualsSpecification()
    {
        // arrange
        ExactSearchBehaviour<TestEntity> behaviour = new();

        // act
        ISpecification<TestEntity> result =
            behaviour.Build("Name", "Bob");

        // assert
        Assert.IsType<PropertyEqualsSpecification<TestEntity>>(result);
    }
}
