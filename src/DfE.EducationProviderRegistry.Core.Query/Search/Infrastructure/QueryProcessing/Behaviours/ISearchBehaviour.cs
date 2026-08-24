using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;

public interface ISearchBehaviour<TEntity>
{
    ISpecification<TEntity> Build(string propertyPath, string value);
}
