using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Abstractions;

internal static class IServiceCollectionFactory
{
    internal static IServiceCollection CreateDefault() => new ServiceCollection();
}
