using DfE.EducationProviderRegistry.Core.Query.Groups;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Group;

internal static class GetGroupsServiceCollectionExtensions
{
    internal static IServiceCollection AddGetGroups(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddSharedFeatureDependencies(configuration)
            .AddGroupsUseCaseDependencies()
            .AddGroupsInfrastructureDependencies();

        return services;
    }
}
