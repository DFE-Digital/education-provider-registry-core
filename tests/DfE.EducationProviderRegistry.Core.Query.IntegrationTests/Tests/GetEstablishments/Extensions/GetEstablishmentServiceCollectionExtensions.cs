using DfE.EducationProviderRegistry.Core.Query.Establishments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.GetEstablishments.Extensions;

internal static class GetEstablishmentServiceCollectionExtensions
{
    internal static IServiceCollection AddGetEstablishments(this
        IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddSharedFeatureDependencies(configuration)
            .AddEstablishmentsUseCaseDependencies()
            .AddEstablishmentsInfrastructureDependencies();

        return services;
    }
}
