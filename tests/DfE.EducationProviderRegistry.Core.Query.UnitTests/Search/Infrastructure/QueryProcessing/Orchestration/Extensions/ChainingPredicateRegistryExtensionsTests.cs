using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.Extensions;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Behaviours.Specifications.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration.Extensions;

public sealed class ChainingPredicateRegistryExtensionsTests
{
    [Fact]
    public void Chain_NullRegistry_Throws()
    {
        // arrange
        ISpecification<TestEntity> right =
            new SpecificationStub<TestEntity>("Name", "Bob");

        // act / assert
        Assert.Throws<ArgumentNullException>(() =>
            ChainingPredicateRegistryExtensions.Chain(
                null!,
                null,
                right,
                "And"));
    }

    [Fact]
    public void Chain_NullPredicateName_ReturnsRight()
    {
        // arrange
        ChainingPredicateRegistry<TestEntity> registry =
            new([]);

        ISpecification<TestEntity> right =
            new SpecificationStub<TestEntity>("Name", "Bob");

        // act
        ISpecification<TestEntity> result =
            registry.Chain(null, right, null);

        // assert
        Assert.Same(right, result);
    }

    [Fact]
    public void Chain_WhitespacePredicateName_ReturnsRight()
    {
        // arrange
        ChainingPredicateRegistry<TestEntity> registry =
            new([]);

        ISpecification<TestEntity> right =
            new SpecificationStub<TestEntity>("Name", "Bob");

        // act
        ISpecification<TestEntity> result =
            registry.Chain(null, right, " ");

        // assert
        Assert.Same(right, result);
    }

    [Fact]
    public void Chain_NullLeft_ReturnsRight()
    {
        // arrange
        ChainingPredicateRegistry<TestEntity> registry =
            new(new()
            {
                ["And"] = (left, right) => left
            });

        ISpecification<TestEntity> right =
            new SpecificationStub<TestEntity>("Name", "Bob");

        // act
        ISpecification<TestEntity> result =
            registry.Chain(null, right, "And");

        // assert
        Assert.Same(right, result);
    }

    [Fact]
    public void Chain_ValidPredicate_UsesResolvedCombiner()
    {
        // arrange
        ISpecification<TestEntity> left =
            new SpecificationStub<TestEntity>("Name", "Bob");

        ISpecification<TestEntity> right =
            new SpecificationStub<TestEntity>("Address.Postcode", "DL1");

        ISpecification<TestEntity> combined =
            new SpecificationStub<TestEntity>("Name", "Combined");

        ChainingPredicateRegistry<TestEntity> registry =
            new(new()
            {
                ["And"] = (_, _) => combined
            });

        // act
        ISpecification<TestEntity> result =
            registry.Chain(left, right, "And");

        // assert
        Assert.Same(combined, result);
    }
}
