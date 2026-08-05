namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Configuration;

public sealed class SearchConfiguration
{
    public IReadOnlyList<SearchIndexKeyConfiguration> Keys { get; init; } = [];
}

