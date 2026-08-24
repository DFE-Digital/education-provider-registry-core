namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

public sealed class SearchConfiguration
{
    public IReadOnlyList<SearchIndexKeyConfiguration> Keys { get; init; } = [];
}

