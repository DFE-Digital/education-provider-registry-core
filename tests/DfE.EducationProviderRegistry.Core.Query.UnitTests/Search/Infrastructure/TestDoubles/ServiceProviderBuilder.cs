using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Core.Query.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class ServiceProviderBuilder
{
    public static IServiceProvider BuildServiceProvider()
    {
        ServiceCollection services = new();

        Dictionary<string, string> configValues =
            new()
            {
                {
                    "FilterKeyToFilterExpressionMapOptions:FilterChainingLogicalOperator",
                    "AndLogicalOperator"
                },
                {
                    "FilterKeyToFilterExpressionMapOptions:SearchFilterToExpressionMap:t.establishment_type_id:FilterExpressionKey",
                    "SingleOrMultiValueEqualsExpression"
                },
                {
                    "FilterKeyToFilterExpressionMapOptions:SearchFilterToExpressionMap:t.establishment_type_id:FilterExpressionValuesDelimiter",
                    ""
                },
                {
                    "FilterKeyToFilterExpressionMapOptions:Map:establishmenttypeid",
                    "SingleOrMultiValueEqualsExpression"
                },
                {
                    "eprweb_eprdat_dotnet_db_connection",
                    "Host=test;Port=5432;Database=test;Username=test;Password=test"
                }
            };

        IConfiguration configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(configValues!)
                .Build();

        services.AddSingleton(configuration);
        services.AddInfraSearchDependencies(configuration);
        services.AddInfraSearchFilterDependencies(configuration);

        return services.BuildServiceProvider(validateScopes: true);
    }
}
