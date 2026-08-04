using System.Linq.Expressions;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Pipeline.Steps;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram;
using DfE.EducationProviderRegistry.Core.Query.Shared.Pipeline;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search;

public sealed class CompositionRootTests
{
    [Fact]
    public void CompositionRoot_Registers_FacetSelectorDictionary()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        // act
        Dictionary<string, Expression<Func<Establishment, object>>> selectors =
            provider.GetRequiredService<Dictionary<string, Expression<Func<Establishment, object>>>>();

        // assert
        Assert.True(selectors.ContainsKey("type"));
    }

    [Fact]
    public void CompositionRoot_Registers_SearchOrchestrator()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        ISearchOrchestrator<Establishment> orchestrator =
            scope.ServiceProvider.GetRequiredService<ISearchOrchestrator<Establishment>>();

        // assert
        Assert.IsType<TrigramSearchOrchestrator<Establishment>>(orchestrator);
    }

    [Fact]
    public void CompositionRoot_Registers_ProjectionBuilder()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        ISearchProjectionBuilder<Establishment> builder =
            scope.ServiceProvider.GetRequiredService<ISearchProjectionBuilder<Establishment>>();

        // assert
        Assert.IsType<EstablishmentSearchProjectionBuilder>(builder);
    }

    [Fact]
    public void CompositionRoot_Registers_SearchProvider()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        ISearchProvider<Establishment> providerInstance =
            scope.ServiceProvider.GetRequiredService<ISearchProvider<Establishment>>();

        // assert
        Assert.IsType<EstablishmentsSearchProvider>(providerInstance);
    }

    [Fact]
    public void CompositionRoot_Registers_FacetProvider()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        IFacetProvider facetProvider =
            scope.ServiceProvider.GetRequiredService<IFacetProvider>();

        // assert
        Assert.IsType<EstablishmentFacetProvider>(facetProvider);
    }

    [Fact]
    public void CompositionRoot_Registers_PipelineSteps()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        IEnumerable<IEvaluationHandler<SearchPipelineContext>> steps =
            scope.ServiceProvider.GetServices<IEvaluationHandler<SearchPipelineContext>>();

        // assert
        Assert.Contains(steps, step => step is SearchOrderMapStep);
        Assert.Contains(steps, step => step is SearchOrderingStep);
        Assert.Contains(steps, step => step is ParallelMappingStep);
        Assert.Contains(steps, step => step is FacetQueryDispatchStep);
        Assert.Contains(steps, step => step is FacetQueryResolverStep);
        Assert.Contains(steps, step => step is FacetQueryBuilderStep);
        Assert.NotNull(scope.ServiceProvider.GetService<Query.Shared.Pipeline.IEvaluator<SearchPipelineContext>>());
    }

    [Fact]
    public void CompositionRoot_Registers_SearchServiceAdapter()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets> adapter =
            scope.ServiceProvider.GetRequiredService<
                ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>>();

        // assert
        Assert.IsType<EstablishmentsSearchServiceAdapter>(adapter);
    }
}
