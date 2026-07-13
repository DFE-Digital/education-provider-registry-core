using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search;

public sealed class SearchEstablishmentByNameReturnsResults : UseCaseIntegrationTestBase
{
    public SearchEstablishmentByNameReturnsResults(IServiceProvider testServicesProvider) : base(testServicesProvider)
    {
    }

    protected override void ConfigureApplicationServices(IServiceCollection services, IConfiguration configuration) => services.AddSearch(configuration);

    [Fact]
    public async Task Returns_Results()
    {
    }
}
