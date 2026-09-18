using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data;
using DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseSeedServices(this IServiceCollection services)
    {
        services.AddSingleton<ISeedDataHandler<IEnumerable<SearchAggregate>, SearchableAggregates>, SearchAggregateCollectionSeedHandler>();
        return services;
    }
}
