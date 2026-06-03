using DfE.EducationProviderRegistry.Core.Query.Establishments;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.GetEstablishments;

internal static class GetEstablishmentServiceCollectionExtensions
{
    internal static IServiceCollection AddGetEstablishments(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddEstablishmentsUseCaseDependencies()
            .AddEstablishmentsInfrastructureDependencies();

        return services;
    }
}
