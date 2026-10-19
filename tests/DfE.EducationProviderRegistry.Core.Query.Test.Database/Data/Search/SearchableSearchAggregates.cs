using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;

public sealed record SearchableSearchAggregates
{
    public required IReadOnlyCollection<SearchAggregate> SearchAggregates { get; init; }

    public static SearchableSearchAggregates Empty() => new()
    {
        SearchAggregates = []
    };
}
