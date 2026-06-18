using DfE.Core.Libraries.CleanArchitecture.Application;
using DfE.EducationProviderRegistry.Core.Query.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Request;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tests.Shared.Services;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search;

public sealed class CompositionRootAddSearchTests
{
    [Fact]
    public void AddSearchDependencies_ShouldThrow_WhenServicesIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CompositionRoot.AddSearchDependencies(null!));
    }

    [Fact]
    public void AddSearchDependencies_ShouldRegisterCorrectDependencyDescriptors()
    {
        // Arrange
        IServiceCollection services = ServiceCollectionDefault.Create();

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "SearchCriteria:SearchFields:0", "Name" },
                { "SearchCriteria:Facets:0", "Name" }
            })
            .Build();

        services.AddSingleton(configuration);

        // act
        IServiceCollection updated = services.AddSearchDependencies();
        IServiceProvider provider = updated.BuildServiceProvider();

        // assert
        updated.ShouldContain<
            ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>,
            DummySearchServiceAdapter>(ServiceLifetime.Scoped);

        updated.ShouldContain<
            IUseCase<SearchRequest, UseCaseResponse<SearchResponse>>,
            SearchUseCase>(ServiceLifetime.Scoped);

        SearchCriteria criteria = provider.GetRequiredService<SearchCriteria>();

        Assert.NotNull(criteria.SearchFields);
        Assert.Single(criteria.SearchFields);
        Assert.Equal("Name", criteria.SearchFields[0]);

        Assert.NotNull(criteria.Facets);
        Assert.Single(criteria.Facets);
        Assert.Equal("Name", criteria.Facets[0]);
    }
}
