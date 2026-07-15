using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;

namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

internal interface ISearchEstablishmentFactory
{
    Task<SearchableEstablishmentsResponse> CreateManyAsync(
        int totalToCreate,
        string searchTerm,
        SearchByNameTerms matches,
        CancellationToken ct = default);
}

public sealed record SearchableEstablishmentsResponse
{
    public required IReadOnlyCollection<Establishment> SearchTermMatches { get; init; }
}

public sealed record SearchByNameTerms(IReadOnlyList<string> matchingNames)
{
    public static SearchByNameTerms Create(
        string searchTerm,
        int matchCount)
    {
        return new SearchByNameTerms(
        [
            .. Enumerable.Range(1, matchCount)
                .Select(number => $"{searchTerm}-{number}")
        ]);
    }
}
