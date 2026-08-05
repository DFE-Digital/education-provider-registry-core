namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Configuration;

public sealed class IndexedFieldConfiguration
{
    public string FieldName { get; init; } = string.Empty;

    public string? ChainingPredicate { get; init; }

    public IReadOnlyList<SearchBehaviourConfiguration> SearchBehaviours { get; init; } = [];
}
