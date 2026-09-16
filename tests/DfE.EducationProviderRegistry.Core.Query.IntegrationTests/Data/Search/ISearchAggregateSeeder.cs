using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

internal interface ISearchAggregateSeeder
{
    Task ClearAsync(CancellationToken ct = default);
    Task<SearchableAggregates> SeedAsync(IReadOnlyCollection<SearchAggregate> searchAggregates, CancellationToken ct = default);
}

public sealed record SearchableAggregates
{
    public required IReadOnlyCollection<SearchAggregate> SearchAggregates { get; init; }
}
