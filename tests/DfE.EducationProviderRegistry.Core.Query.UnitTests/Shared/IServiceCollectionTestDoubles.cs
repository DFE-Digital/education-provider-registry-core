using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Shared;

internal static class IServiceCollectionTestDoubles
{
    internal static IServiceCollection Default() => new ServiceCollection();
}
