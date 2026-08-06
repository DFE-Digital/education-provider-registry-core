using DfE.Core.Libraries.DesignPatterns.Specification;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Behaviours.Specifications;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Behaviours;

internal sealed class PartialSearchBehaviour<TEntity> : ISearchBehaviour<TEntity>
{
    public string Name => "partial";

    public ISpecification<TEntity> Build(string propertyPath, string value)
        => new IlikeSpecification<TEntity>(propertyPath, value);
}
