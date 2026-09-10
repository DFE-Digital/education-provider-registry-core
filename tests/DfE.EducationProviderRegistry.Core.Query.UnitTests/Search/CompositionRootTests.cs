using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Projections;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram;
using DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search;

public sealed class CompositionRootTests
{
    [Fact]
    public void CompositionRoot_Registers_FacetDefinitionDictionary()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        // act
        Dictionary<string, FacetDefinition<Establishment>> definitions =
            provider.GetRequiredService<Dictionary<string, FacetDefinition<Establishment>>>();

        // assert
        Assert.True(definitions.ContainsKey("establishmenttypeid"));
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
        Assert.IsType<SearchAggregateProjectionBuilder>(builder);
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
        Assert.IsType<SearchProvider>(providerInstance);
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
        Assert.IsType<FacetProvider>(facetProvider);
    }

    [Fact]
    public void CompositionRoot_Registers_SearchServiceAdapter()
    {
        // arrange
        IServiceProvider provider =
            ServiceProviderBuilder.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();

        // act
        ISearchServiceAdapter<SearchProviderResults, SearchFacets> adapter =
            scope.ServiceProvider.GetRequiredService<
                ISearchServiceAdapter<SearchProviderResults, SearchFacets>>();

        // assert
        Assert.IsType<SearchServiceAdapter>(adapter);
    }
}
