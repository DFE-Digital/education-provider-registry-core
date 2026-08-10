namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;

public sealed class SearchBehaviourRegistry<TEntity>
{
    private readonly Dictionary<string, ISearchBehaviour<TEntity>> _behaviours;

    public SearchBehaviourRegistry(IEnumerable<ISearchBehaviour<TEntity>> behaviours)
    {
        _behaviours =
            behaviours.ToDictionary(
                behaviour => behaviour.Name,
                StringComparer.OrdinalIgnoreCase);
    }

    public ISearchBehaviour<TEntity> Get(string name) => _behaviours[name];
}
