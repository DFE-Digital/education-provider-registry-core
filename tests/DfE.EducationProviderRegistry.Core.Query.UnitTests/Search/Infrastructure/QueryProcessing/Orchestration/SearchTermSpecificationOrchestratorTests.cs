using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration.TestDoubles;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;
using Microsoft.Extensions.Options;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.Orchestration;

public sealed class SearchTermSpecificationOrchestratorTests
{
    [Fact]
    public void Orchestrate_Throws_WhenKeyNotConfigured()
    {
        // arrange
        SearchConfiguration config = SearchConfigurationStub.CreateDefault();
        IOptions<SearchConfiguration> options = Options.Create(config);

        Mock<ISearchIndexFieldSpecificationOrchestrator<TestEntity>> indexFieldMock =
            SearchIndexFieldSpecificationOrchestratorTestDouble.Mock<TestEntity>();

        Mock<IChainingPredicateRegistry<TestEntity>> predicateRegistryMock =
            ChainingPredicateRegistryTestDouble.Mock<TestEntity>();

        SearchTermSpecificationOrchestrator<TestEntity> orchestrator =
            new(
                indexFieldMock.Object,
                predicateRegistryMock.Object,
                options);

        // act/assert
        Assert.Throws<KeyNotFoundException>(() =>
            orchestrator.Orchestrate("UnknownKey", "Bob"));
    }

    [Fact]
    public void Orchestrate_CallsIndexFieldOrchestrator_ForEachField()
    {
        // arrange
        SearchConfiguration config = SearchConfigurationStub.CreateDefault();
        IOptions<SearchConfiguration> options = Options.Create(config);

        ISpecification<TestEntity> fieldSpec =
            new SpecificationStub<TestEntity>(entity => entity.Name == "Bob");

        Mock<ISearchIndexFieldSpecificationOrchestrator<TestEntity>> indexFieldMock =
            SearchIndexFieldSpecificationOrchestratorTestDouble.Create<TestEntity>(
                "Name",
                fieldSpec);

        Mock<IChainingPredicateRegistry<TestEntity>> predicateRegistryMock =
            ChainingPredicateRegistryTestDouble.CreateAnd<TestEntity>();

        SearchTermSpecificationOrchestrator<TestEntity> orchestrator =
            new(
                indexFieldMock.Object,
                predicateRegistryMock.Object,
                options);

        // act
        ISpecification<TestEntity> result = orchestrator.Orchestrate("Name", "Bob");

        // assert/verify
        Assert.True(result.IsSatisfiedBy(new TestEntity { Name = "Bob" }));

        indexFieldMock.Verify();
        predicateRegistryMock.Verify(chainingPredicateRegistry =>
            chainingPredicateRegistry.Resolve("AND"), Times.Once);
    }

    [Fact]
    public void Orchestrate_ChainsMultipleFieldsCorrectly()
    {
        // arrange
        SearchConfiguration config = SearchConfigurationStub.CreatePerson();
        IOptions<SearchConfiguration> options = Options.Create(config);

        ISpecification<TestEntity> nameSpec =
            new SpecificationStub<TestEntity>(entity => entity.Name == "Bob");

        ISpecification<TestEntity> ageSpec =
            new SpecificationStub<TestEntity>(entity => entity.Age == 30);

        Mock<ISearchIndexFieldSpecificationOrchestrator<TestEntity>> indexFieldMock =
            SearchIndexFieldSpecificationOrchestratorTestDouble.Create<TestEntity>(
                new Dictionary<string, ISpecification<TestEntity>>
                {
                    { "Name", nameSpec },
                    { "Age", ageSpec }
                });

        Mock<IChainingPredicateRegistry<TestEntity>> predicateRegistryMock =
            ChainingPredicateRegistryTestDouble.CreateAnd<TestEntity>();

        SearchTermSpecificationOrchestrator<TestEntity> orchestrator =
            new(
                indexFieldMock.Object,
                predicateRegistryMock.Object,
                options);

        // act
        ISpecification<TestEntity> result = orchestrator.Orchestrate("Person", "Bob");

        // assert/verify
        Assert.True(result.IsSatisfiedBy(new TestEntity { Name = "Bob", Age = 30 }));
        Assert.False(result.IsSatisfiedBy(new TestEntity { Name = "Bob", Age = 20 }));

        indexFieldMock.Verify();
        predicateRegistryMock.Verify(chainingPredicateRegistry =>
            chainingPredicateRegistry.Resolve("AND"), Times.Exactly(2));
    }
}
