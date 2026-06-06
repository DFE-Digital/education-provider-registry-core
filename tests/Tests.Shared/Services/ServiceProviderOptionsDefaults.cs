using Microsoft.Extensions.DependencyInjection;

namespace Tests.Shared.Services;

public static class ServiceProviderOptionsDefaults
{
    public static ServiceProviderOptions Default => new()
    {
        ValidateOnBuild = true,
        ValidateScopes = true
    };
}
