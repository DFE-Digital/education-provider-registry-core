namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Configuration;

public sealed class SearchIndexKeyConfiguration
{
    public string SearchTermKey { get; init; } = string.Empty;

    public string? ChainingPredicate { get; set; }

    public IReadOnlyList<IndexedFieldConfiguration> IndexedFields { get; init; } = [];
}
