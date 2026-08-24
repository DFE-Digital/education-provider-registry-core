namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Configuration;

public sealed class IndexedFieldConfiguration
{
    public string FieldName { get; init; } = string.Empty;

    public string? DefaultBehaviourChainingPredicate { get; init; }

    public IReadOnlyList<SearchBehaviourConfiguration> SearchBehaviours { get; init; } = [];
}
