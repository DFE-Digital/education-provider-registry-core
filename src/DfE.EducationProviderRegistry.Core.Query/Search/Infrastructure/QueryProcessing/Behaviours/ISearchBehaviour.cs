using DfE.Core.Libraries.DesignPatterns.Specification;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;

public interface ISearchBehaviour<TEntity>
{
    string Name { get; }

    ISpecification<TEntity> Build(string propertyPath, string value);
}
