namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

public sealed class SearchIndexKeyConfiguration
{
    public string SearchTermKey { get; init; } = string.Empty;

    public string? FieldChainingPredicate { get; set; }

    public IReadOnlyList<IndexedFieldConfiguration> IndexedFields { get; init; } = [];
}
