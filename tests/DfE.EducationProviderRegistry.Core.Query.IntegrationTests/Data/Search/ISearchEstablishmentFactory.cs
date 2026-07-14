using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

public interface ISearchEstablishmentFactory
{
    Task<IReadOnlyCollection<Establishment>> CreateManyAsync(
        int totalToCreate,
        int matchingSearchTermCount,
        string searchTerm,
        CancellationToken ct = default);
}
