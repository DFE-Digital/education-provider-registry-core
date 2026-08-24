using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;

public sealed class SearchBehaviourRegistry<TEntity> :
    ISearchBehaviourRegistry<TEntity> where TEntity : class
{
    private readonly ReadOnlyDictionary<string, ISearchBehaviour<TEntity>> _behaviours;

    public SearchBehaviourRegistry(
        IEnumerable<
            KeyValuePair<
                string,
                ISearchBehaviour<TEntity>>> behaviours)
    {
        _behaviours =
            behaviours.ToDictionary(
                behaviour => behaviour.Key,
                behaviours => behaviours.Value,
                StringComparer.OrdinalIgnoreCase).AsReadOnly();
    }

    public ISearchBehaviour<TEntity> Get(string name) => _behaviours[name];
}
