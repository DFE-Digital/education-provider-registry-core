using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Search;

public sealed record SearchableAggregates
{
    public required IReadOnlyCollection<SearchAggregate> SearchAggregates { get; init; }
}
