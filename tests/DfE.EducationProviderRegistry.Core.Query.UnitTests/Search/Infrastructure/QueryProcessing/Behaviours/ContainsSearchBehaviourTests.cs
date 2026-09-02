using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours;

public sealed class ContainsSearchBehaviourTests
{
    [Fact]
    public void Build_ReturnsContainsSpecification()
    {
        // arrange
        ContainsSearchBehaviour<TestEntity> behaviour = new();

        // act
        ISpecification<TestEntity> result =
            behaviour.Build("Name", "Bob");

        // assert
        Assert.IsType<ContainsSpecification<TestEntity>>(result);
    }
}
