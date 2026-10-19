using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Groups;

public sealed record class GroupAggregates
{
    public GroupAggregates(IEnumerable<GroupAggregate>? values)
    {
        Values = values?.ToList();
    }

    public IReadOnlyList<GroupAggregate>? Values { get; }
}
