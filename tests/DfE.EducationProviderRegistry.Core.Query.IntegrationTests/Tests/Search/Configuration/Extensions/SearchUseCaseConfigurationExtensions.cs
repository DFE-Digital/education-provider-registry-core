using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration.Extensions;

public static class SearchUseCaseConfigurationExtensions
{
    public static IConfigurationBuilder AddSearchUseCaseConfiguration(this IConfigurationBuilder builder, Action<SearchUseCaseConfigurationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        SearchUseCaseConfigurationBuilder target = SearchUseCaseConfigurationBuilder.Create();
        configure.Invoke(target);
        IReadOnlyDictionary<string, string?> output = target.Build();
        builder.AddInMemoryCollection(output);
        return builder;
    }
}
