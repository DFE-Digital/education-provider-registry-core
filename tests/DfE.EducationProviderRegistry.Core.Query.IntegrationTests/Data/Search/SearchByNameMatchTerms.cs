namespace DfE.EducationProviderRegistry.Core.Query.IntegrationTests.Data.Search;

public sealed record SearchByNameMatchTerms(IReadOnlyList<string> matchingNames)
{
    public static SearchByNameMatchTerms Create(
        string searchTerm,
        int matchCount)
    {
        return new SearchByNameMatchTerms(
        [
            .. Enumerable.Range(1, matchCount)
                .Select(number => $"{searchTerm}-{number}")
        ]);
    }
}
