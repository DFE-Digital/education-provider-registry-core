using DfE.EducationProviderRegistry.Core.Query.Establishments;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
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
            // TODO temporary until we have custom logger-provider set up in application-logging, COULD register IObservationHandler? on logging ITestOutputHelper / IMessageSink
            .AddLogging()
            .AddEstablishmentsUseCaseDependencies()
            .AddEstablishmentsInfrastructureDependencies();

        services.AddDbContext<EducationProviderRegistryDbContext>(option =>
            option.UseNpgsql(configuration["eprweb-eprdat-dotnet-db-connection"]));

        return services;
    }
}
