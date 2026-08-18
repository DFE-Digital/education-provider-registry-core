using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;
using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;

internal static class SearchConfigurationExtensions
{
    public static IConfigurationBuilder AddSearchConfiguration(
        this IConfigurationBuilder builder,
        (string termKey, IEnumerable<Action<IndexedFieldConfigurationBuilder>> fieldsConfigure) configure)
    {
        IndexedFieldConfiguration[] fields =
            [.. configure.fieldsConfigure.Select(fieldConfigure =>
                {
                    IndexedFieldConfigurationBuilder builder = IndexedFieldConfigurationBuilder.Create();
                    fieldConfigure.Invoke(builder);
                    return builder.Build();
                })];

        Dictionary<string, string?> configuration =
            SearchConfigurationBuilder.Create()
                .WithBehaviourForSearchTerm(configure.termKey, fields)
                .Build();

        builder.AddInMemoryCollection(configuration);

        return builder;
    }
}
