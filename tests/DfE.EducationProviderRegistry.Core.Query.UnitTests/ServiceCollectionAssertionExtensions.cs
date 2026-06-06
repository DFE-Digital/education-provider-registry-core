using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests;

public static class ServiceCollectionAssertionExtensions
{
    public static ServiceDescriptor ShouldContain<TService, TImpl>(
        this IServiceCollection services, ServiceLifetime lifetime)
    {
        ServiceDescriptor descriptor =
            Assert.Single(services, d => d.ServiceType == typeof(TService));

        Assert.Equal(typeof(TImpl), descriptor.ImplementationType);

        Assert.Equal(lifetime, descriptor.Lifetime);

        return descriptor;
    }
}
