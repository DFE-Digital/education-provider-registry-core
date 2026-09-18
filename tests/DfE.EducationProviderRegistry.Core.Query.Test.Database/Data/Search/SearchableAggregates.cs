using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Search;

public sealed record SearchableAggregates
{
    public required IReadOnlyCollection<SearchAggregate> SearchAggregates { get; init; }

    public static SearchableAggregates Empty() => new()
    {
        SearchAggregates = []
    };
}
