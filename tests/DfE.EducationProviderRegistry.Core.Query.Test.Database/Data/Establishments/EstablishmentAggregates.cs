using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Test.Database.Data.Establishments;

public sealed record EstablishmentAggregates
{
    public EstablishmentAggregates(IEnumerable<EstablishmentAggregate>? values)
    {
        Values = values?.ToList();
    }

    public IReadOnlyCollection<EstablishmentAggregate>? Values { get; }
}
