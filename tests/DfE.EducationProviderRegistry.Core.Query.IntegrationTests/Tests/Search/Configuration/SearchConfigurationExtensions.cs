using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;
using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Tests.Search.Configuration;

internal static class SearchConfigurationExtensions
{
    public static IConfigurationBuilder AddSearchConfiguration(
        this IConfigurationBuilder builder,
        (string termKey, string chainFieldsWithPredicate, IEnumerable<Action<IndexedFieldConfigurationBuilder>> fieldsConfigure)[] termBehaviours)
    {
        SearchConfigurationBuilder configBuilder = SearchConfigurationBuilder.Create();

        foreach ((string termKey, string chainFieldsWithPredicate, IEnumerable < Action<IndexedFieldConfigurationBuilder>> fieldsConfigure) in termBehaviours)
        {
            IndexedFieldConfiguration[] fields =
            [.. fieldsConfigure.Select(fieldConfigure =>
                {
                    IndexedFieldConfigurationBuilder builder = IndexedFieldConfigurationBuilder.Create();
                    fieldConfigure.Invoke(builder);
                    return builder.Build();
                })];

            configBuilder.WithBehaviourForSearchTerm(termKey, fields, fieldChainingPredicate: chainFieldsWithPredicate);
        }
        
        Dictionary<string, string?> configuration = configBuilder.Build();

        builder.AddInMemoryCollection(configuration);

        return builder;
    }
}
