using System.Diagnostics.CodeAnalysis;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchAggregateTestBuilder
{
    private static long _nextId = 1;

    public static SearchAggregate Create(
        string providerId,
        string name,
        string typeName,
        long typeId)
    {
        return new SearchAggregate
        {
            SearchAggregateId = _nextId++,
            ProviderId = providerId,
            ProviderName = name,
            ProviderTypeName = typeName,
            ProviderTypeId = typeId
        };
    }
}
