using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours.Specifications;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.QueryProcessing.Behaviours;

internal sealed class ExactSearchBehaviour<TEntity> : ISearchBehaviour<TEntity>
{
    public ISpecification<TEntity> Build(string propertyPath, string value)
        => new PropertyEqualsSpecification<TEntity>(propertyPath, value);
}
