using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration.Extensions;

public static class SearchUseCaseConfigurationExtensions
{
    public static IConfigurationBuilder AddSearchUseCaseConfiguration(this IConfigurationBuilder builder, Action<SearchUseCaseConfigurationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        SearchUseCaseConfigurationBuilder configuration = SearchUseCaseConfigurationBuilder.Create();
        configure.Invoke(configuration);
        IReadOnlyDictionary<string, string?> output = configuration.Build();
        builder.AddInMemoryCollection(output);
        return builder;
    }
}
