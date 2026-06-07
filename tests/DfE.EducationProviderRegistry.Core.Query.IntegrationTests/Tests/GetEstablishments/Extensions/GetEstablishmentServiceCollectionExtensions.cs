using DfE.EducationProviderRegistry.Core.Query.Establishments;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.GetEstablishments.Extensions;

internal static class GetEstablishmentServiceCollectionExtensions
{
    internal static IServiceCollection AddGetEstablishments(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            // TODO temporary until we have custom logger-provider set up in application-logging, COULD register IObservationHandler? on logging ITestOutputHelper / IMessageSink
            .AddLogging()
            .AddEstablishmentsUseCaseDependencies()
            .AddEstablishmentsInfrastructureDependencies();

        return services;
    }
}
