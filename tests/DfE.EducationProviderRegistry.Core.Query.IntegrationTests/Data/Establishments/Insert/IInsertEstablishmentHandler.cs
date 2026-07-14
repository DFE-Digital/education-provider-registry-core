using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Establishments.Insert;

public interface IInsertEstablishmentHandler
{
    Task InsertAsync(
        IReadOnlyCollection<Establishment> establishments,
        CancellationToken ct);
}
