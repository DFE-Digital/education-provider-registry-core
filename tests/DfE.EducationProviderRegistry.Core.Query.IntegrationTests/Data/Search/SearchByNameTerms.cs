namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

public sealed record SearchByNameTerms(IReadOnlyList<string> matchingNames)
{
    public static SearchByNameTerms Create(
        string searchTerm,
        int matchCount,
        string? termSuffix = null)
    {
        return new SearchByNameTerms(
        [
            .. Enumerable.Range(1, matchCount).Select(number => $"{searchTerm}{termSuffix ?? $"-{number}"}")
        ]);
    }
}
