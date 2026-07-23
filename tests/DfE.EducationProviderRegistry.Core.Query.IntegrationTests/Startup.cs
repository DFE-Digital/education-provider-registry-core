using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Extensions;
using DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using MartinCostello.Logging.XUnit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.DependencyInjection.Logging;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests;

#pragma warning disable CA1822
// Mark members as static - Startup is instantiated by XUnit.DependencyInjection and instance is expected

public class Startup
{
    public void ConfigureHost(IHostBuilder hostBuilder) =>
            hostBuilder
                .ConfigureHostConfiguration(builder => { })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    builder.AddJsonFile($"appsettings.{context.HostingEnvironment}.json", optional: true, reloadOnChange: true);
                });

    public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
    {
        services.AddOptions<XUnitLoggerOptions>();

        services.AddLogging((loggingBuilder) =>
            loggingBuilder.AddXunitOutput((optionsConfigure) =>
            {
                // TODO filter logging
            }));

        services.AddPostgresDatabase(context.Configuration);
    }
}
