using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

internal interface ISearchEstablishmentFactory
{
    Task<SearchableEstablishmentsResponse> CreateManyAsync(
        int totalToCreate,
        SearchByNameTerms matches,
        CancellationToken ct = default);
}

public sealed record SearchableEstablishmentsResponse
{
    public required IReadOnlyCollection<Establishment> SearchTermMatches { get; init; }
}
