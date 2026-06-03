using Microsoft.Extensions.Configuration;

namespace IntegrationTests.Abstractions;

public static class DefaultConfigurationBuilder
{
    public static IConfigurationBuilder Create() => new ConfigurationBuilder();
}

