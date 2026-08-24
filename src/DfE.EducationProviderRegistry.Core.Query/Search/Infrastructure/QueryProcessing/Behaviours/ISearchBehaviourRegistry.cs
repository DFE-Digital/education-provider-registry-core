namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;

public interface ISearchBehaviourRegistry<TEntity>
    where TEntity : class
{
    ISearchBehaviour<TEntity> Get(string name);
}

