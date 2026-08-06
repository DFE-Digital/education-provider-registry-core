using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Behaviours.Specifications;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Behaviours;

internal sealed class ExactSearchBehaviour<TEntity> : ISearchBehaviour<TEntity>
{
    public string Name => "exact";

    public ISpecification<TEntity> Build(string propertyPath, string value)
        => new PropertyEqualsSpecification<TEntity>(propertyPath, value);
}
