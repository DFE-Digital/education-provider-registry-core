namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Configuration;

public sealed class SearchBehaviourConfiguration
{
    public string Name { get; init; } = string.Empty;

    public string? ChainingPredicate { get; init; }
}
