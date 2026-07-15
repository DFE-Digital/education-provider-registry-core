namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

internal interface ISearchEstablishmentFactory
{
    Task<SearchableEstablishments> CreateManyAsync(
        int totalToCreate,
        string searchTerm,
        SearchByNameMatchTerms matches,
        CancellationToken ct = default);
}
