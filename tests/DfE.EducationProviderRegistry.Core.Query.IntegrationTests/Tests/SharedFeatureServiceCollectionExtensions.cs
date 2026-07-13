using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests;

internal static class SharedFeatureServiceCollectionExtensions
{
    internal static IServiceCollection AddSharedFeatureDependencies(this
        IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            // TODO temporary until we have custom logger-provider set up in application-logging, COULD register IObservationHandler? on logging ITestOutputHelper / IMessageSink
            .AddLogging();

        services.AddDbContextFactory<EducationProviderRegistryDbContext>(
            (option) =>
                option.UseNpgsql(configuration["eprweb_eprdat_dotnet_db_connection"]));

        return services;
    }
}
