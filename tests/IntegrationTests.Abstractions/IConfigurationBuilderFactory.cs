using Microsoft.Extensions.Configuration;

namespace IntegrationTests.Abstractions;

internal static class IConfigurationBuilderFactory
{
    internal static IConfigurationBuilder CreateDefault() => new ConfigurationBuilder();
}

