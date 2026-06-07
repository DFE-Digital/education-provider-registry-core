using Microsoft.Extensions.DependencyInjection;

namespace Tests.Shared.Services;

public static class ServiceCollectionDefault
{
    public static IServiceCollection Create() => new ServiceCollection();
}
