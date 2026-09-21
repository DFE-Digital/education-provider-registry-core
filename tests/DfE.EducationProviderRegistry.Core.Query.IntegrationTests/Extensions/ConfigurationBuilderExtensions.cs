using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Extensions;

internal static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddPostgresConnection(this IConfigurationBuilder builder, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        return builder.AddInMemoryCollection(new Dictionary<string, string?>()
        {
            ["eprweb_eprdat_dotnet_db_connection"] = connectionString
        });
    }
}
