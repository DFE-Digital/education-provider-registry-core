using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Orchestration.SpecificationChaining;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing.TestDoubles;
using Moq;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.QueryProcessing;

public sealed class SearchQueryProcessorTests
{
    [Fact]
    public void ProcessSearch_NoValidTerms_ReturnsOriginalQuery()
    {
        // arrange
        Dictionary<(string Key, string Value), ISpecification<TestEntity>> specs = [];

        Mock<ISearchTermSpecificationOrchestrator<TestEntity>> orchestratorMock =
            SearchTermSpecificationOrchestratorTestDouble.Create(specs);

        Mock<IChainingPredicateRegistry<TestEntity>> registryMock =
            ChainingPredicateRegistryTestDouble.CreateAnd<TestEntity>();

        SearchQueryProcessor<TestEntity> processor =
            new(orchestratorMock.Object, registryMock.Object);

        IQueryable<TestEntity> query =
            new List<TestEntity>
            {
                new() { Name = "Bob", Age = 30 }
            }
            .AsQueryable();

        // act
        IQueryable<TestEntity> result = processor.ProcessSearch(query, null);

        // assert/verify
        Assert.Same(query, result);
        orchestratorMock.VerifyNoOtherCalls();
        registryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ProcessSearch_FiltersOutInvalidTerms()
    {
        // arrange
        Dictionary<(string Key, string Value), ISpecification<TestEntity>> specs =
            new()
            {
                {
                    ("Name", "Bob"),
                    new SpecificationStub<TestEntity>(entity => entity.Name == "Bob")
                }
            };

        Mock<ISearchTermSpecificationOrchestrator<TestEntity>> orchestratorMock =
            SearchTermSpecificationOrchestratorTestDouble.Create(specs);

        Mock<IChainingPredicateRegistry<TestEntity>> registryMock =
            ChainingPredicateRegistryTestDouble.CreateAnd<TestEntity>();

        SearchQueryProcessor<TestEntity> processor =
            new(orchestratorMock.Object, registryMock.Object);

        SearchTerm?[] terms =
        [
            new SearchTerm("Name","Bob"),
            new SearchTerm("", "Invalid"),
            null
        ];

        IQueryable<TestEntity> query =
            new List<TestEntity>
            {
                new() { Name = "Bob" },
                new() { Name = "Alice" }
            }
            .AsQueryable();

        // act
        IQueryable<TestEntity> result = processor.ProcessSearch(query, terms);

        // assert/verify
        Assert.Single(result);
        Assert.Equal("Bob", result.First().Name);
        orchestratorMock.Verify(orchestrator =>
            orchestrator.Orchestrate("Name", "Bob"), Times.Once);
        registryMock.Verify(registry =>
            registry.Resolve("AND"), Times.Once);
    }

    [Fact]
    public void ProcessSearch_SingleTerm_AppliesSpecification()
    {
        // arrange
        Dictionary<(string Key, string Value), ISpecification<TestEntity>> specs =
            new()
            {
                {
                    ("Age", "40"),
                    new SpecificationStub<TestEntity>(entity => entity.Age == 40)
                }
            };

        Mock<ISearchTermSpecificationOrchestrator<TestEntity>> orchestratorMock =
            SearchTermSpecificationOrchestratorTestDouble.Create(specs);

        Mock<IChainingPredicateRegistry<TestEntity>> registryMock =
            ChainingPredicateRegistryTestDouble.CreateAnd<TestEntity>();

        SearchQueryProcessor<TestEntity> processor =
            new(orchestratorMock.Object, registryMock.Object);

        SearchTerm[] terms =
        [
            new SearchTerm("Age","40")
        ];

        IQueryable<TestEntity> query =
            new List<TestEntity>
            {
                new() { Age = 40 },
                new() { Age = 20 }
            }
            .AsQueryable();

        // act
        IQueryable<TestEntity> result = processor.ProcessSearch(query, terms);

        // assert/verify
        Assert.Single(result);
        Assert.Equal(40, result.First().Age);
        orchestratorMock.Verify(orchestrator =>
            orchestrator.Orchestrate("Age", "40"), Times.Once);
        registryMock.Verify(registry =>
            registry.Resolve("AND"), Times.Once);
    }

    [Fact]
    public void ProcessSearch_MultipleTerms_AppliesAndChaining()
    {
        // arrange
        Dictionary<(string Key, string Value), ISpecification<TestEntity>> specs =
            new()
            {
                {
                    ("Name", "Bob"),
                    new SpecificationStub<TestEntity>(entity => entity.Name == "Bob")
                },
                {
                    ("Age", "30"),
                    new SpecificationStub<TestEntity>(entity => entity.Age == 30)
                }
            };

        Mock<ISearchTermSpecificationOrchestrator<TestEntity>> orchestratorMock =
            SearchTermSpecificationOrchestratorTestDouble.Create(specs);

        Mock<IChainingPredicateRegistry<TestEntity>> registryMock =
            ChainingPredicateRegistryTestDouble.CreateAnd<TestEntity>();

        SearchQueryProcessor<TestEntity> processor =
            new(orchestratorMock.Object, registryMock.Object);

        SearchTerm[] terms =
        [
            new SearchTerm("Name","Bob"),
            new SearchTerm("Age","30")
        ];

        IQueryable<TestEntity> query =
            new List<TestEntity>
            {
                new() { Name = "Bob", Age = 30 },
                new() { Name = "Bob", Age = 20 },
                new() { Name = "Alice", Age = 30 }
            }
            .AsQueryable();

        // act
        IQueryable<TestEntity> result = processor.ProcessSearch(query, terms);

        // assert/verify
        Assert.Single(result);
        Assert.Equal("Bob", result.First().Name);
        Assert.Equal(30, result.First().Age);

        orchestratorMock.Verify(orchestrator =>
            orchestrator.Orchestrate("Name", "Bob"), Times.Once);
        orchestratorMock.Verify(orchestrator =>
            orchestrator.Orchestrate("Age", "30"), Times.Once);
        registryMock.Verify(registry =>
            registry.Resolve("AND"), Times.Exactly(2));
    }

    [Fact]
    public void ProcessSearch_NoMatchingResults_ReturnsEmpty()
    {
        Dictionary<(string Key, string Value), ISpecification<TestEntity>> specs =
            new()
            {
                {
                    ("Name", "Charlie"),
                    new SpecificationStub<TestEntity>(entity => entity.Name == "Charlie")
                }
            };

        Mock<ISearchTermSpecificationOrchestrator<TestEntity>> orchestratorMock =
            SearchTermSpecificationOrchestratorTestDouble.Create(specs);

        Mock<IChainingPredicateRegistry<TestEntity>> registryMock =
            ChainingPredicateRegistryTestDouble.CreateAnd<TestEntity>();

        SearchQueryProcessor<TestEntity> processor =
            new(orchestratorMock.Object, registryMock.Object);

        SearchTerm[] terms =
        [
            new SearchTerm("Name","Charlie")
        ];

        IQueryable<TestEntity> query =
            new List<TestEntity>
            {
                new() { Name = "Bob" },
                new() { Name = "Alice" }
            }
            .AsQueryable();

        IQueryable<TestEntity> result = processor.ProcessSearch(query, terms);

        Assert.Empty(result);

        orchestratorMock.Verify(orchestrator =>
            orchestrator.Orchestrate("Name", "Charlie"), Times.Once);
        registryMock.Verify(registry =>
            registry.Resolve("AND"), Times.Once);
    }
}
