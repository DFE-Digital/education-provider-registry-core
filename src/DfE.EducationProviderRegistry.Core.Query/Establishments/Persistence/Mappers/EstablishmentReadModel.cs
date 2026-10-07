using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.Establishments.Persistence.Mappers;

public sealed record class EstablishmentReadModel
{
    public EstablishmentReadModel(EstablishmentAggregate establishment, IReadOnlyCollection<EstablishmentGovernorAggregate> governors)
    {
        ArgumentNullException.ThrowIfNull(establishment);
        Establishment = establishment;
        Governors = governors ?? [];
    }

    public EstablishmentAggregate Establishment { get; }
    public IReadOnlyCollection<EstablishmentGovernorAggregate> Governors { get; }
}
