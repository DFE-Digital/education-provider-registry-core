using Microsoft.Extensions.Configuration;

namespace Tests.Shared;

public static class ConfigurationDefault
{
    public static IConfiguration Create() => CreateBuilder().Build();
    public static IConfigurationBuilder CreateBuilder() => new ConfigurationBuilder();
}
