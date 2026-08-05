namespace DfE.EducationProviderRegistry.Core.Query.UnitTests.Search.Infrastructure.Behaviours;

public sealed class SearchBehaviourRegistry
{
    private readonly Dictionary<string, ISearchBehaviour> _behaviours;

    public SearchBehaviourRegistry(IEnumerable<ISearchBehaviour> behaviours)
    {
        _behaviours = behaviours.ToDictionary(
            behaviour =>
                behaviour.Name,
                StringComparer.OrdinalIgnoreCase);
    }

    public ISearchBehaviour Get(string name) => _behaviours[name];
}
