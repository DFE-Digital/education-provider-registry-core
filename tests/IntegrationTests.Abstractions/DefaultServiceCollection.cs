using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Abstractions;

internal static class DefaultServiceCollection
{
    internal static IServiceCollection Create() => new ServiceCollection();
}
