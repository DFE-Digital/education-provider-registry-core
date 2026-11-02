using DfE.EducationProviderRegistry.Core.Query.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.Options;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search;

public sealed class CompositionRootAddInfrastructureSearchDependenciesTests
{
    [Fact]
    public void AddInfraSearchDependencies_BindsFacetOptions()
    {
        // arrange
        Dictionary<string, string?> configValues =
            new()
            {
                {
                    "FacetOptions:Definitions:searchprovidertypename:ValueProperty",
                    nameof(SearchAggregate.ProviderTypeName)
                },
                {
                    "FacetOptions:Definitions:searchprovidertypename:LabelProperty",
                    nameof(SearchAggregate.ProviderTypeName)
                }
            };

        IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configValues)
                .Build();

        ServiceCollection services = new();

        services.AddInfraSearchDependencies(configuration);

        ServiceProvider provider =
            services.BuildServiceProvider();

        // act
        FacetOptions options =
            provider.GetRequiredService<FacetOptions>();

        // assert
        Assert.True(
            options.Definitions.TryGetValue(
                "searchprovidertypename",
                out FacetDefinition? definition));

        Assert.NotNull(definition);
        Assert.Equal(
            nameof(SearchAggregate.ProviderTypeName),
            definition.ValueProperty);
        Assert.Equal(
            nameof(SearchAggregate.ProviderTypeName),
            definition.LabelProperty);
    }
}
