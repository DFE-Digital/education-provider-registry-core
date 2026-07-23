using DfE.EducationProviderRegistry.Core.Query.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Extensions;

internal static class SearchServiceCollectionExtensions
{
    internal static IServiceCollection AddSearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSharedFeatureDependencies(configuration);

        services.AddApplicationSearchDependencies(configuration);
        services.AddInfraSearchDependencies(configuration);
        services.AddInfraSearchFilterDependencies();
        return services;
    }
}
