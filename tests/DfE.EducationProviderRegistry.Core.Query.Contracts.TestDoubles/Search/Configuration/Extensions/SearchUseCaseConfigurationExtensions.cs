using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.Contracts.TestDoubles.Search.Configuration.Extensions;

public static class SearchUseCaseConfigurationExtensions
{
    public static IConfigurationBuilder AddSearchUseCaseConfiguration(this IConfigurationBuilder builder, Action<SearchUseCaseConfigurationBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        SearchUseCaseConfigurationBuilder useCaseConfigurationBuilder = SearchUseCaseConfigurationBuilder.Create();
        configure.Invoke(useCaseConfigurationBuilder);
        IReadOnlyDictionary<string, string?> output = useCaseConfigurationBuilder.Build();
        return builder.AddInMemoryCollection(output);
    }
}
